using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;
        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }
        [HttpGet]
        [Route("get-all-managers")]
        public async Task<IActionResult> GetAllManagers()
        {
            var response = await _managerService.GetAllManagerAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-staff-detail")]
        //Role User
        public async Task<IActionResult> GetManagerDetail([FromQuery] int managerId)
        {
            var admin = await _managerService.GetManagerDetailByIdAsync(managerId);
            if (admin.IsSuccess)
            {
                return Ok(admin);
            }
            return BadRequest(admin);
        }
        [HttpPost]
        [Route("create-manager")]
        public async Task<IActionResult> CreateManager([FromBody] ManagerCM managerCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _managerService.CreateManagerAsync(managerCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-manager")]
        public async Task<IActionResult> UpdateManager([FromQuery] int managerId, [FromBody] ManagerUM managerUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _managerService.UpdateManagerAsync(managerId, managerUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        [Route("convert-staff-to-manager")]
        public async Task<IActionResult> ConvertManagerToStaff(int userId, int roleId, int branchId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _managerService.ConvertStaffToManager(userId, roleId, branchId);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete]
        [Route("delete-admin")]
        public async Task<ActionResult<User>> DeleteManager([FromQuery] int managerId)
        {
            var response = await _managerService.DeleteManagerAsync(managerId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
