using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using _2Sport_BE.DataContent;
using _2Sport_BE.ViewModels;
using AutoMapper;
using _2Sport_BE.Service.Services;
using System.Text;
using System.Security.Cryptography;
using _2Sport_BE.Helpers;

namespace _2Sport_BE.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenService _refreshTokenService;
        public UserController(
            IUserService userService,
            IMapper mapper,
            IRefreshTokenService refreshTokenService
            )
        {
            _userService = userService;
            _mapper = mapper;
            _refreshTokenService = refreshTokenService;
        }
        [HttpGet]
        [Route("get-all-users")]
        public async Task<IActionResult> GetAllUser(string? fullName, string? username)
        {
            try
            {
                var query = await _userService.GetAllAsync();
                if(!string.IsNullOrWhiteSpace(fullName))
                {
                    fullName = fullName.ToLower();
                    query = query.Where(x => x.FullName.ToLower().Contains(fullName));
                }
                if (!string.IsNullOrWhiteSpace(username))
                {
                    username = username.ToLower();
                    query = query.Where(x => x.UserName.ToLower().Contains(fullName));
                }

                var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
        }
        [HttpGet]
        [Route("get-users-by-role")]
        public async Task<IActionResult> GetUsesByRole(int roleId)
        {
            try
            {
                var query = await _userService.GetAsync(_ => _.RoleId == roleId);
                var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserDetail(int userId)
        {
            try
            {
                var user = await _userService.FindAsync(userId);
                var tokenUser = await _refreshTokenService.GetTokenDetail(userId);
                return Ok(new { User = user, Token = tokenUser });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCM userCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<UserCM, User>(userCM);
                user.Password = HashPassword(userCM.Password);
                user.CreatedDate = DateTime.Now;
                user.RoleId = 1;
                user.IsActive = true;
                await _userService.AddAsync(user);
                _userService.Save();
                return StatusCode(201, new { processStatus = "Success", userId = user.Id }); ;
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUM userUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userService.FindAsync(id);
                if (user == null)
                {
                    return BadRequest(new { processStatus = "NotExisted" });
                }
                user.FullName = userUM.FullName;
                user.Salary = userUM.Salary;
                user.Email = userUM.Email;
                user.BirthDate = userUM.BirthDate;
                user.Gender = userUM.Gender;
                user.Phone = userUM.Phone;

               await _userService.UpdateAsync(user);
               return Ok(new { processStatus = "Success", data = user.Id });
            }
            catch (Exception e)
            {
                //Duplicate
                if (e is DbUpdateException dbUpdateEx)
                {
                    return BadRequest(new { processStatus = "Duplicate" });
                }
                return BadRequest(e);
            }

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _userService.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            await _userService.RemoveAsync(id);
            return await Task.FromResult(user);
        }
        [HttpPut("change-status/{id}")]
        public async Task<IActionResult> ChangeStatusUser(int id)
        {
            try
            {
                var user = await _userService.FindAsync(id);
                if (user == null)
                {
                    return BadRequest(new { processStatus = "Not Existed" });
                }
                user.IsActive = !user.IsActive;
                await _userService.UpdateAsync(user);
                _userService.Save();
                return Ok(new { processStatus = "Success", data = id });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Route("getCurrentUser")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var UserId = GetCurrentUserIdFromToken();
            var result = await _userService.GetAsync(_ => _.Id == UserId);
            return Ok(result);
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

    }
}
