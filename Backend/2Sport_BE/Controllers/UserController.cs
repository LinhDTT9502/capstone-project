using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using IMailService = _2Sport_BE.Services.IMailService;
using _2Sport_BE.Infrastructure.DTOs;
using Hangfire.Common;
using CloudinaryDotNet.Actions;
using _2Sport_BE.Enums;
using _2Sport_BE.Infrastructure.Helpers;
using _2Sport_BE.Services.Caching;
using _2Sport_BE.Service.Services.Caching;
using _2Sport_BE.ViewModels;
using Twilio.Types;
using NuGet.Protocol.Plugins;

namespace _2Sport_BE.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMailService _mailService;
        private readonly IImageService _imageService;
        private readonly IPhoneNumberService _phoneNumberService;
        private readonly IMethodHelper _methodHelper;
        private readonly IRedisCacheService _redisCacheService;
        private readonly string _phoneNumberOTPsKey;

        public UserController(
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            IMailService mailService,
            IImageService imageService,
            IPhoneNumberService phoneNumberService,
            IMethodHelper methodHelper,
            IRedisCacheService redisCacheService,
            IConfiguration configuration
            )                                       

        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _mailService = mailService;
            _imageService = imageService;
            _phoneNumberService = phoneNumberService;
            _methodHelper = methodHelper;
            _redisCacheService = redisCacheService;
            _phoneNumberOTPsKey = configuration.GetValue<string>("RedisKeys:PhoneNumberOtps");
        }
        [HttpGet]
        [Route("get-all-users")]
        //Role Admin
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var response = await _userService.GetAllUsers();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("search")]
        //Role Admin
        public async Task<IActionResult> SearchUser(string? fullName, string? username)
        {
            try
            {
                var response = await _userService.SearchUsers(fullName, username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("get-users-detail")]
        //Role User
        public async Task<IActionResult> GetUserDetails([FromQuery] int userId)
        {
            try
            {
                var user = await _userService.GetUserDetailsById(userId);
                var tokenUser = await _refreshTokenService.GetTokenDetail(userId);
                return Ok(new { User = user, Token = tokenUser });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet]
        [Route("get-users-without-branch/{roleId}")]
        //Role User
        public async Task<IActionResult> GetUsersWithoutBranch(int roleId)
        {
            try
            {
                var user = await _userService.GetByRoleUsersWithoutBranch(roleId);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(email) && _.Token.Equals(token))).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Email or Token are invalid!");
            }
            if (user.EmailConfirmed)
            {
                return Ok("Your email verified!");
            }
            user.EmailConfirmed = true;
            user.Token = null;
            await _userService.UpdateUserAsync(user.Id, user);
            return Ok(new { Message = "Email verified successfully." });
        }

        [HttpGet]
        [Route("get-profile")]
        //Role User
        public async Task<IActionResult> GetProfile([FromQuery] int userId)
        {
            try
            {
                var user = await _userService.GetUserDetailsById(userId);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> AddNewUser([FromBody] UserCM userCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.CreateUserAsync(userCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-user")]
        //Role Admin
        public async Task<IActionResult> EditUserAsync([FromQuery] int id, [FromBody] UserUM userUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.UpdateUserAsync(id, userUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-profile")]
        //Role Customer
        public async Task<IActionResult> EditProfileAsync([FromQuery] int id, [FromBody] ProfileUM profileUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.UpdateProfile(id, profileUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        
        [HttpPut]
        [Route("update-password/{userId}")]
        //Role Customer
        public async Task<IActionResult> EditPasswordAsync([FromQuery] int userId, [FromBody] ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.UpdatePasswordAsync(userId, changePasswordVM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        [Route("send-otp-to-email/{userId}")]
        public async Task<IActionResult> SendOtpToMail(int userId, [FromQuery] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            if (!_mailService.IsValidEmail(email))
            {
                return BadRequest("Email is invalid format!");
            }
            var user = await _userService.GetUserById(userId);
            if (user is null)
            {
                return BadRequest("User is not found!");
            }
            var otp = _methodHelper.GenerateOTPCode();
            var claims = new Dictionary<string, string>
                            {
                                { "UserId", userId.ToString() },
                                { "Email", email },
                                { "OTP", otp }
                            };
            var jwt = _methodHelper.GenerateJwtForStrings(claims);


            var isSent = await _mailService.SenOTPMaillAsync(email, otp);

            if (isSent)
            {
                return Ok(new { Message = "OTP for Changing email is sent successfully.", Token = jwt });
            }
            return StatusCode(500, "Unable to OTP for Changing email! Please try again later.");

        }
        [HttpPut]
        [Route("update-email/{userId}")]
        //Role Customer
        public async Task<IActionResult> EditEmailAsync(int userId, [FromBody] ResetEmailRequesrt resetEmailRequesrt)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.UpdateEmailAsync(userId, resetEmailRequesrt);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        [Route("upload-avatar")]
        public async Task<IActionResult> EditAvatar(AvatarModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userService.GetUserById(model.userId);
            if (user != null)
            {
                if (model.Avatar != null)
                {
                    var uploadResult = await _imageService.UploadImageToCloudinaryAsync(model.Avatar);
                    if (uploadResult != null && uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        user.ImgAvatarPath = uploadResult.SecureUrl.AbsoluteUri;
                        await _userService.UpdateUserAsync(user.Id, user);
                        return Ok("Upload avatar successfully");
                    }
                    else
                    {
                        return BadRequest("Something wrong!");
                    }
                }
                else
                {
                    user.ImgAvatarPath = "";
                }
            }
            return BadRequest(model);
        }

        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] SendEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email are required.");
            }

            if (!_mailService.IsValidEmail(request.Email))
            {
                return BadRequest("Email is invalid!");
            }

            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(request.Email))).FirstOrDefault();

            if (user is null)
            {
                return BadRequest("Email is not found!");
            }

            var token = await _mailService.GenerateEmailVerificationTokenAsync(request.Email);
            user.Token = token;
            await _userService.UpdateUserAsync(user.Id, user);

            var verificationLink = Url.Action("VerifyEmail", "User", new { token = token, email = user.Email }, Request.Scheme);

            var result = await _mailService.SendVerifyEmailAsync(verificationLink, user.Email);
            if (result)
            {
                return Ok(new { Message = "Verification email sent." });
            }
            return BadRequest("Can not create mail");
        }

        [HttpPut]
        [Route("send-sms-otp/{phoneNumber}")]
        public async Task<IActionResult> SendSmsOTP (string phoneNumber)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }

                var user = await _userService.GetUserById(userId);
                if (user is null)
                {
                    return BadRequest("Not found user!");
                }

                var otp = GenerateOTP();
                
                var isSuccess = await _phoneNumberService.SendSmsToPhoneNumber(otp, phoneNumber);
                if (isSuccess == (int)Errors.NotExcepted)
                {
                    return StatusCode(500, "Something wrong!");
                }
                
                var listPhoneNumberOTPsInCache = _redisCacheService.GetData<List<PhoneNumberOtp>>(_phoneNumberOTPsKey)
                                                                        ?? new List<PhoneNumberOtp>();
                var existedPhoneNumberOTP = listPhoneNumberOTPsInCache.Find(_ => _.PhoneNumber == phoneNumber);
                if (existedPhoneNumberOTP is not null)
                {
                    existedPhoneNumberOTP.OTP = otp;
                    _redisCacheService.SetData(_phoneNumberOTPsKey, listPhoneNumberOTPsInCache, TimeSpan.FromMinutes(5));
                } else
                {
                    listPhoneNumberOTPsInCache.Add(new PhoneNumberOtp()
                    {
                        OTP = otp,
                        PhoneNumber = phoneNumber
                    });
                    _redisCacheService.SetData(_phoneNumberOTPsKey, listPhoneNumberOTPsInCache, TimeSpan.FromMinutes(5));
                }
                
                return Ok("Send OTP to your number");

            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("verify-phone-number/{otp}")]
        public async Task<IActionResult> VerifyPhoneNumber(int otp)
        {
            try
            {
                var userId = GetCurrentUserIdFromToken();

                if (userId == 0)
                {
                    return Unauthorized();
                }
                var user = await _userService.GetUserById(userId);

                var listPhoneNumberOTPsInCache = _redisCacheService.GetData<List<PhoneNumberOtp>>(_phoneNumberOTPsKey)
                                                                        ?? new List<PhoneNumberOtp>();

                var existedPhoneNumberOTP = listPhoneNumberOTPsInCache.Find(_ => _.PhoneNumber == user.PhoneNumber);
                if (existedPhoneNumberOTP is null)
                {
                    return BadRequest("Mã OTP không hợp lệ!");
                }
                if (existedPhoneNumberOTP.OTP != otp)
                {
                    return BadRequest("Mã OTP không hợp lệ!");
                }
                user.PhoneNumberConfirmed = true;
                await _userService.UpdateUserAsync(userId, user);
                existedPhoneNumberOTP.OTP = 0;
                _redisCacheService.SetData(_phoneNumberOTPsKey, listPhoneNumberOTPsInCache, TimeSpan.FromMinutes(5));
                return Ok("Verify phone number successfully!");
            } catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("edit-phone-number")]
        public async Task<IActionResult> EditPhoneNumber(string newPhoneNumber, int otp)
        {
            var userId = GetCurrentUserIdFromToken();

            if (userId == 0)
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);

            var listPhoneNumberOTPsInCache = _redisCacheService.GetData<List<PhoneNumberOtp>>(_phoneNumberOTPsKey)
                                                                    ?? new List<PhoneNumberOtp>();

            var existedPhoneNumberOTP = listPhoneNumberOTPsInCache.Find(_ => _.PhoneNumber == newPhoneNumber);
            if (existedPhoneNumberOTP is null)
            {
                return BadRequest("Mã OTP không hợp lệ!");
            }
            if (existedPhoneNumberOTP.OTP != otp)
            {
                return BadRequest("Mã OTP không hợp lệ!");
            }
            user.PhoneNumber = newPhoneNumber;
            user.PhoneNumberConfirmed = true;
            existedPhoneNumberOTP.OTP = 0;
            _redisCacheService.SetData(_phoneNumberOTPsKey, listPhoneNumberOTPsInCache, TimeSpan.FromMinutes(5));
            await _userService.UpdateUserAsync(userId, user);
            return Ok("Edit phone number succsessfuly");

        }

        [HttpPut]
        [Route("edit-status/{userId}")]
        public async Task<IActionResult> EditStatus(int userId, [FromBody] bool status)
        {
            var response = await _userService.UpdateStatus(userId, status);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete]
        [Route("delete-user")]
        //Role Admin
        public async Task<ActionResult<User>> RemoveUser([FromQuery] int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [NonAction]
        protected int GetCurrentUserIdFromToken()
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
                return UserId;
            }
            catch
            {
                return UserId;
            }
        }

        [NonAction]
        private int GenerateOTP()
        {
            Random random = new Random();
            int otpNumber = random.Next(100000, 1000000); // Generates a number between 100000 and 999999
            return otpNumber;
        }
    }
}