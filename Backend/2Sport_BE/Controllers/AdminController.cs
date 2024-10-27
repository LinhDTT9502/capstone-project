using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    //Admin nay lay employee, user, 
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IAdminService _adminService;
        public AdminController(
            IUserService userService,
            IMapper mapper,
            IAdminService adminService
            )
        {
            _userService = userService;
            _mapper = mapper;
            _adminService = adminService;
        }

        [HttpGet]
        [Route("get-all-admins")]
        public async Task<IActionResult> GetAllAddmins()
        {
            var response = await _adminService.GetAllAdminAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-admin-detail")]
        //Role User
        public async Task<IActionResult> GetAdminDetail([FromQuery] int adminId)
        {
            var admin = await _adminService.GetAdminDetailAsync(adminId);
            if (admin.IsSuccess)
            {
                return Ok(admin);
            }
            return BadRequest(admin);
        }
        [HttpPost]
        [Route("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminCM adminCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _adminService.CreateAdminAsync(adminCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-admin")]
        public async Task<IActionResult> UpdateAdminAsync([FromQuery] int admidId, [FromBody] AdminUM adminUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _adminService.UpDateAdminAsync(admidId, adminUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete]
        [Route("delete-admin")]
        public async Task<ActionResult<User>> DeleteAdmin([FromQuery] int adminId)
        {
            var response = await _adminService.DeleteAdminAsync(adminId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
