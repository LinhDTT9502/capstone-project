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
        public async Task<IActionResult> GetAllStaffs()
        {
            var response = await _staffService.GetAllStaffsAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-staff-detail")]
        //Role User
        public async Task<IActionResult> GetStaffDetail([FromQuery] int staffId)
        {
            var admin = await _staffService.GetStaffDetailAsync(staffId);
            if (admin.IsSuccess)
            {
                return Ok(admin);
            }
            return BadRequest(admin);
        }
        [HttpPost]
        [Route("create-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCM staffCM)
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
        [Route("update-staff")]
        public async Task<IActionResult> UpdateStaff([FromQuery] int staffId, [FromBody] StaffUM staffUM)
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
        [HttpDelete]
        [Route("delete-admin")]
        public async Task<ActionResult<User>> DeleteStaff([FromQuery] int staffId)
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
