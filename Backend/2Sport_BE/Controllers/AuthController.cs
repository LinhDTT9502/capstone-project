using _2Sport_BE.API.Services;
using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Service.Services;
using _2Sport_BE.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using _2Sport_BE.Services;
using _2Sport_BE.Service.Enums;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISendMailService _mailService;
        private readonly ICartService _cartService;

        public AuthController(
            IUserService userService,
            IIdentityService identityService,
            IRefreshTokenService refreshTokenService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ISendMailService mailService,
            ICartService cartService)
        {
            _userService = userService;
            _identityService = identityService;
            _refreshTokenService = refreshTokenService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
            _cartService = cartService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromForm] MailRequest mailRequest)
        {
            

            mailRequest.Subject = mailRequest.Subject;
            mailRequest.Body = mailRequest.Body;
            mailRequest.ToEmail = mailRequest.ToEmail;
            mailRequest.Attachments = mailRequest.Attachments;
            if (mailRequest == null)
            {
                return BadRequest("Email request is null");
            }
            var result = await _mailService.SendEmailAsync(mailRequest);
            if (result)
            {
            return Ok("Email sent successfully");
            }
            else
            {
                return BadRequest();
            }
        }
        
        [Route("sign-in")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserLogin loginModel)
        {
            var password = HashPassword(loginModel.Password);
            loginModel.Password = password;
            var result = await _identityService.LoginAsync(loginModel);
            if (result.IsSuccess)
            {
                var cart = await _cartService.GetCartByUserId((int)result.Data.UserId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = result.Data.UserId,
                        CartItems = new List<CartItem>(),
                        User = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Id == result.Data.UserId),
                    };

                    await _cartService.AddNewCart(cart);
                }
            }
            return Ok(result);
        }

        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshAsync([FromBody] TokenModel request)
        {
            var result = await _identityService.RefreshTokenAsync(request);
            return Ok(result);
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> LogOutAsync([FromBody] TokenModel request)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetObjectAsync(_ => _.Token == request.RefreshToken);
            if (token == null)
            {
                return BadRequest("Not found token!");
            }
            else
            {
                await _refreshTokenService.RemoveToken(token);
                _unitOfWork.Save();
                return Ok("Query Successfully");
            }
        }

        [HttpGet("oauth-login")]
        public IActionResult ExternalLogin1()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/Auth/signin-google" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLogin()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();

            var name = response.Principal.FindFirstValue(ClaimTypes.Name);
            var email = response.Principal.FindFirstValue(ClaimTypes.Email);
            var phone = response.Principal.FindFirstValue(ClaimTypes.MobilePhone);
            var gender = response.Principal.FindFirstValue(ClaimTypes.Gender);

            if (email == null)
            {
                return BadRequest("Error retrieving Google user information");
            }

            ResponseModel<TokenModel> result = new ResponseModel<TokenModel>();
            var user = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Email == email);
            if (user != null)
            {
                result = await _identityService.LoginGoogleAsync(user);
                var cart = await _cartService.GetCartByUserId(user.Id);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.Id,
                        CartItems = new List<CartItem>(),
                        User = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Id == user.Id),
                    };

                    await _cartService.AddNewCart(cart);
                }
            }
            else
            {
                user = new User()
                {
                    FullName = name,
                    Email = email,
                    Phone = phone,
                    CreatedDate = DateTime.Now,
                    RoleId = 4,
                    Gender = gender,
                    IsActive = true,
                };
                await _userService.AddAsync(user);
                _unitOfWork.Save();

                result = await _identityService.LoginGoogleAsync(user);
            }

            var token = result.Data.Token;
            var refreshToken = result.Data.RefreshToken;

            
            var script = $@"
                <script>
                    window.opener.postMessage({{
                        token: '{token}',
                        refreshToken: '{refreshToken}'
                    }}, 'http://localhost:5173');
                    window.close();
                </script>";

            return Content(script, "text/html");
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> CreateUser([FromBody] UserCM userCM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Email.ToLower() == userCM.Email.ToLower()) != null)
                {
                    return StatusCode(500, new { processStatus = "Already have an account!" });
                }

                var user = _mapper.Map<UserCM, User>(userCM); 
                user.Password = HashPassword(userCM.Password);
                user.CreatedDate = DateTime.Now;
                user.RoleId = (int) UserRole.Customer;
                user.IsActive = true;
                await _userService.AddAsync(user);
                var cart = await _cartService.GetCartByUserId(user.Id);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.Id,
                        CartItems = new List<CartItem>(),
                        User = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Id == user.Id),
                    };

                    await _cartService.AddNewCart(cart);
                }
                _userService.Save();
                return StatusCode(201, new { processStatus = "Success", userId = user.Id }); ;
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException dbUpdateEx)
                {
                    return BadRequest(new { processStatus = "User is duplicated" });
                }
                return BadRequest(ex);
            }

        }
        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] UserCM userCM, int roleId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var role = await _unitOfWork.RoleRepository.GetObjectAsync(_ => _.Id == roleId);
                var user = await _userService.GetAsync(_ => _.Email == userCM.Email);
                if(user == null)
                {
                    User staff = new User()
                    {
                        UserName = userCM.Username,
                        Email = userCM.Email,
                        CreatedDate = DateTime.Now,
                        Password = HashPassword(userCM.Password),
                        FullName = userCM.FullName,
                        RoleId = roleId,
                        Role = role,
                        IsActive = true
                    };
                    await _userService.AddAsync(staff);
                    return StatusCode(201, new { processStatus = "Success", userId = staff.Id });
                }
                return StatusCode(500, new { processStatus = $"Already have an account!" });
            }
            catch (Exception ex)
            {
                //Duplicate
                if (ex is DbUpdateException dbUpdateEx)
                {
                    return BadRequest(new { processStatus = "Duplicate" });
                }
                return BadRequest(ex);
            }

        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordVM model)
        {
            var user = await GetUserFromToken();
            if(user is null)
            {
                return Unauthorized("Invalid user");
            }
            else
            {
                if (!user.Password.Equals(HashPassword(model.OldPassword)))
                {
                    return BadRequest("Old passwords do not match");
                }
                user.Password = HashPassword(model.NewPassword);
                _unitOfWork.Save();
                return Ok("Password changed successfully");
            } 
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotVM forgotVM)
        {
            try
            {
                var username = forgotVM.Username;
                var mail = forgotVM.Email;
                //check
                var check = await _unitOfWork.UserRepository.GetObjectAsync(_ => _.Email == mail && _.UserName == _.UserName);
                if(check != null)
                {
                    var newPassword = GenerateRandomString(6);
                    check.Password = HashPassword(newPassword);
                    _unitOfWork.Save();
                    //Send mail to get a new password
                    MailRequest mailRequest = new MailRequest();
                    mailRequest.Subject = "Request to change a new password from TwoSport";
                    mailRequest.Body = $"We are the administrators of TwoSport. Your new password is {newPassword}. Best Regards!";
                    mailRequest.ToEmail = mail;
                    var imailservice = _mailService;
                    var result = await _mailService.SendEmailAsync(mailRequest);
                    

                    if(result)
                    {
                        return Ok(new { Message = "Query successfully", IsSuccess = true });
                    }
                    else
                    {
                        return Ok(new { Message = "Query failed", IsSuccess = false });
                    }
                   
                }
                else
                {
                    return BadRequest(new { Message = "Invalid Information!", IsSuccess = false });
                }
                
            }catch(Exception ex)
            {

            }
            return BadRequest(new {Message = "Some thing wrongs", IsSuccess = false });
        }
        [NonAction]
        public string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        [NonAction]
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }

        [NonAction]
        private async Task<User> GetUserFromToken()
        {
            int UserId = 0;
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var identity = HttpContext.User.Identity as ClaimsIdentity;
                    if (identity != null)
                    {
                        IEnumerable<Claim> claims = identity.Claims;
                        string strUserId = identity.FindFirst("UserId").Value;
                        int.TryParse(strUserId, out UserId);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var user = await _userService.GetAsync(_ => _.Id == UserId);
            return user.FirstOrDefault();
        }
    }
}
