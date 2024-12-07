using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }
        [HttpGet]
        [Route("get-all-staffs")]
        public async Task<IActionResult> ListAllStaffs()
        {
            var response = await _staffService.GetAllStaffsAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-staffs-by-branch")]
        public async Task<IActionResult> GetStaffsByBranch(int branchId)
        {
            var response = await _staffService.GetStaffsByBranchId(branchId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }

        [HttpGet]
        [Route("get-staff-details")]
        //Role User
        public async Task<IActionResult> GetStaffDetail([FromQuery] int staffId)
        {
            var admin = await _staffService.GetStaffDetailsAsync(staffId);
            if (admin.IsSuccess)
            {
                return Ok(admin);
            }
            return BadRequest(admin);
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> AddStaff([FromBody] StaffCM staffCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffService.CreateStaffAsync(staffCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> EditStaff([FromQuery] int staffId, [FromBody] StaffUM staffUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _staffService.UpdateStaffAsync(staffId, staffUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        [Route("convert-manager-to-staff")]
        public async Task<IActionResult> EditManagerToStaff(int userId, int roleId, int branchId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _staffService.ConvertManagerToStaff(userId, roleId, branchId);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete]
        [Route("delete")]
        public async Task<ActionResult<User>> RemoveStaff([FromQuery] int staffId)
        {
            var response = await _staffService.DeleteStaffAsync(staffId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
