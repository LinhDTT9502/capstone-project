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
        public UserController(
            IUserService userService,
            IRefreshTokenService refreshTokenService,
            IMailService mailService,
            IImageService imageService
            )
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _mailService = mailService;
            _imageService = imageService;
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
        public async Task<IActionResult> GetUserDetail([FromQuery] int userId)
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
        public async Task<IActionResult> CreateUser([FromBody] UserCM userCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.AddUserAsync(userCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-user")]
        //Role Admin
        public async Task<IActionResult> UpdateUserAsync([FromQuery] int id, [FromBody] UserUM userUM)
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
        public async Task<IActionResult> UpdateProfileAsync([FromQuery] int id, [FromBody] ProfileUM profileUM)
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
        [Route("update-password")]
        //Role Customer
        public async Task<IActionResult> UpdatePasswordAsync([FromQuery] int id, [FromBody] ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _userService.UpdatePasswordAsync(id, changePasswordVM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpDelete]
        [Route("delete-user")]
        //Role Admin
        public async Task<ActionResult<User>> DeleteUser([FromQuery] int id)
        {
            var response = await _userService.RemoveUserAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
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
        [HttpGet("verify-phone-number")]
        public async Task<IActionResult> VerifyPhoneNumber(string from, string to)
        {
            var response = await _userService.VerifyPhoneNumber(from, to);

            return Ok(response);
        }
        [HttpPost]
        [Route("upload-avatar")]
        public async Task<IActionResult> UpdateAvatar(AvatarModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userService.GetUserById(model.userId);
            if (user != null)
            {
                if(model.Avatar != null)
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
    }
}