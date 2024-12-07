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
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.Helpers;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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
        private readonly IMethodHelper _methodHelper;
        public AuthController(
            IUserService userService,
            IAuthService identityService,
            IRefreshTokenService refreshTokenService,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMailService mailService,
            IMethodHelper methodHelper)
        {
            _userService = userService;
            _identityService = identityService;
            _refreshTokenService = refreshTokenService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
            _methodHelper = methodHelper;
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
            var response = await _identityService.HandleLogoutAsync(request);
           if(response.IsSuccess) return Ok(response);
           return BadRequest(response);
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
                var user = await _unitOfWork.UserRepository
                           .GetObjectAsync(_ =>
                           _.Email.ToLower().Equals(registerModel.Email.ToLower()) ||
                           _.UserName.Equals(registerModel.Username));

                var token = await _mailService.GenerateEmailVerificationTokenAsync(user.Email);
                user.Token = token;
                await _userService.UpdateUserAsync(user.Id, user);

                var verificationLink = Url.Action("VerifyEmail", "User", new { token = token, email = user.Email }, Request.Scheme);

                var isSent = await _mailService.SendVerifyEmailAsync(verificationLink, user.Email);
                if (isSent)
                {
                    return StatusCode(201, new { processStatus = result.Message, userId = result.Data });
                }
            }
            return StatusCode(500, result);
        }
        [Route("refresh-token")]
        [HttpPost]
        public async Task<IActionResult> RefreshAsync([FromBody] TokenModel request)
        {
            var result = await _identityService.RefreshAccessTokenAsync(request);
            return Ok(result);
        }

        [HttpGet("sign-in-google")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Auth", null, "https")
            };

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var response = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (response.Principal == null) return BadRequest();

            var email = response.Principal.FindFirst(ClaimTypes.Email);

            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(email.Value))).FirstOrDefault();

            if (user != null && !user.EmailConfirmed)
            {
                var verifyToken = await _mailService.GenerateEmailVerificationTokenAsync(user.Email);
                user.Token = verifyToken;
                await _userService.UpdateUserAsync(user.Id, user);

                var verificationLink = Url.Action("VerifyEmail", "User", new { token = verifyToken, email = user.Email }, Request.Scheme);
                var isSent = await _mailService.SendVerifyEmailAsync(verificationLink, user.Email);
                if (isSent)
                {
                    return Ok(new { Message = "Your account is not verified. A verification email has been sent. Please check." });
                }
                else
                {

                return BadRequest("Sending verification email is failed!");
                }
            }
            else
            {
                var result = await _identityService.HandleLoginGoogle(response.Principal);
                var token = result.Data.Token;
                var refreshToken = result.Data.RefreshToken;

                ResponseDTO<TokenModel> model = new ResponseDTO<TokenModel>()
                {
                    IsSuccess = true,
                    Message = "Login google successfully",
                    Data = new TokenModel()
                    {
                        Token = token,
                        RefreshToken = refreshToken
                    }
                };
                /*var script = $@"
                <script>
                    window.opener.postMessage({{
                        token: '{token}',
                        refreshToken: '{refreshToken}'
                    }}, 'https://twosportshop.vercel.app/');
                    window.close();
                </script>";*/

                return Ok(model);
            }
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
        [Route("sign-up-mobile")]
        [HttpPost]
        public async Task<IActionResult> SignUpForMobile([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _identityService.SignUpMobileAsync(registerModel);

            if (result.IsSuccess)
            {
                return StatusCode(201, new { processStatus = result.Message, userId = result.Data });
            }
            return StatusCode(500, result);
        }
        [Route("verify-account-mobile")]
        [HttpPost]
        public async Task<IActionResult> VerifyForMobile([FromBody] VerifyOTPMobile verifyOTPModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _unitOfWork.UserRepository
                       .GetObjectAsync(_ =>
                       _.Email.ToLower().Equals(verifyOTPModel.Email.ToLower()) &&
                       _.UserName.Equals(verifyOTPModel.Username));
            if (user is null) return NotFound($"User with {verifyOTPModel.Email} not found");
            if (!user.Token.Equals(verifyOTPModel.OtpCode))
                return BadRequest("The otp is wrong, try again");

            user.Token = null;
            user.EmailConfirmed = true;
            await _userService.UpdateUserAsync(user.Id, user);

            return StatusCode(200, "Verify successfully");
        }
        [HttpPost("forgot-password-request-mobile")]
        public async Task<IActionResult> SendResetPasswordByEmailForMobile([FromBody] SendEmailRequest request)
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
            var otp = _methodHelper.GenerateOTPCode();
            user.PasswordResetToken = otp;
            await _userService.UpdateUserAsync(user.Id, user);

            var isSent = await _mailService.SenOTPMaillAsync(user.Email, otp);

            if (isSent)
            {
                return Ok(new { Message = "Reset password OTP email sent successfully." });
            }
            return StatusCode(500, "Unable to send reset password OTP email. Please try again later.");
        }
        [HttpGet("validate-reset-token-mobile")]
        public async Task<IActionResult> ValidateResetPasswordTokenMobile(string token, string email)
        {
            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(email) && _.PasswordResetToken.Equals(token))).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Invalid or expired token.");
            }

            return Ok("Token is valid.");
        }
        [Route("reset-password-mobile")]
        [HttpPost]
        public async Task<IActionResult>ResetPasswordForMobile([FromBody] ResetPasswordMobile resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_mailService.IsValidEmail(resetPassword.Email))
            {
                return BadRequest("Email is invalid!");
            }

            var user = await _unitOfWork.UserRepository
                       .GetObjectAsync(_ =>
                       _.Email.ToLower().Equals(resetPassword.Email.ToLower()));
            if (user is null) return NotFound($"User with {resetPassword.Email} not found");
            if (!user.PasswordResetToken.Equals(resetPassword.OtpCode))
                return BadRequest("The otp is wrong, try again");

            user.PasswordResetToken = null;
            user.HashPassword = _methodHelper.HashPassword(resetPassword.NewPassword);
            var isUpdated = await _userService.UpdateUserAsync(user.Id, user);
            if(isUpdated.IsSuccess) return StatusCode(200, "Password reset successfully");
            return BadRequest("Password reset failed");
        }
        [HttpPost("send-otp-request-mobile")]
        public async Task<IActionResult> SendOTPByEmailForMobile(string userName, string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
            {
                return BadRequest("Email and Token are required.");
            }

            if (!_mailService.IsValidEmail(email))
            {
                return BadRequest("Email is invalid!");
            }
            var user = (await _userService.GetUserWithConditionAsync(_ => _.Email.Equals(email) && _.UserName.Equals(userName))).FirstOrDefault();
            if (user is null)
            {
                return BadRequest("Email or Username is not found!");
            }
            var otp = _methodHelper.GenerateOTPCode();
            user.Token = otp;
            await _userService.UpdateUserAsync(user.Id, user);

            var isSent = await _mailService.SenOTPMaillAsync(user.Email, otp);

            if (isSent)
            {
                return Ok(new { Message = "Reset password OTP email sent successfully." });
            }
            return StatusCode(500, "Unable to send reset password OTP email. Please try again later.");
        }
    }
}
