using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AutoMapper;
using _2Sport_BE.Services;
using Microsoft.AspNetCore.Authentication.Facebook;
using _2Sport_BE.Infrastructure.DTOs;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IAuthService _identityService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;

        public AuthController(
            IUserService userService,
            IAuthService identityService,
            IRefreshTokenService refreshTokenService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMailService mailService)
        {
            _userService = userService;
            _identityService = identityService;
            _refreshTokenService = refreshTokenService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
        }

        [Route("sign-in")]
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserLogin loginModel)
        {
            if (loginModel is null)
            {
                return BadRequest(new { message = "UserLogin data is required." });
            }

            if (string.IsNullOrEmpty(loginModel.UserName) || string.IsNullOrEmpty(loginModel.Password))
            {
                return BadRequest(new { message = "Username and Password are required." });
            }

            var result = await _identityService.LoginAsync(loginModel);

            return Ok(result);
        }

        [Route("sign-out")]
        [HttpPost]
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

        [Route("sign-up")]
        [HttpPost]
        public async Task<IActionResult> SignUp([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_mailService.IsValidEmail(registerModel.Email))
            {
                return BadRequest("Email is invalid!");
            }

            var result = await _identityService.SignUpAsync(registerModel);

            if (result.IsSuccess)
            {
                return StatusCode(201, new { processStatus = result.Message, userId = result.Data });
            }

            return StatusCode(500, result);
        }

        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshAsync([FromBody] TokenModel request)
        {
            var result = await _identityService.RefreshTokenAsync(request);
            return Ok(result);
        }

        [HttpGet("signin-google")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties { RedirectUri = "api/Auth/google-response" };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();


            var result = await _identityService.HandleLoginGoogle(response.Principal);
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

        //Login Facebook chua xong     
        [HttpGet("signin-facebook")]
        public IActionResult FaceBookLogin()
        {
            var redirectUrl = Url.Action("FacebookResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }
        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest();
            // Lấy thông tin người dùng
            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var name = result.Principal.FindFirstValue(ClaimTypes.GivenName) ??
                                   result.Principal.FindFirstValue(ClaimTypes.Name);
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname);


            return Ok(new
            {
                Email = email,
                Name = name,
                LastName = lastName,
                Claims = claims
            });
        }

        [HttpPost("forgot-password-request")]
        public async Task<IActionResult> SendResetPasswordEmail([FromBody] SendEmailRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email and Token are required.");
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
            user.PasswordResetToken = token;
            await _userService.UpdateUserAsync(user.Id, user);
            var resetLink = Url.Action("ValidateResetToken", "Auth", new { token = token, email = user.Email }, Request.Scheme);

            var result = await _mailService.SendForgotPasswordEmailAsync(resetLink, user.Email);
            if (result)
            {
                return Ok(new { Message = "Reset password email sent successfully." });
            }
            return StatusCode(500, "Unable to send reset password email. Please try again later.");
        }

        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(string token, string email)
        {
            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(email) && _.PasswordResetToken.Equals(token))).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Token is valid.");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email, token, and new password are required.");
            }

            var result = await _identityService.HandleResetPassword(request);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}
