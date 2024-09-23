using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public ManagerController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        [Route("add-attendance")]
        public async Task<IActionResult> AddAttendance([FromBody] AttendanceCM attendanceCM)
        {
            var response = await _attendanceService.AddAttendance(attendanceCM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        [Route("update-attendance")]
        public async Task<IActionResult> UpdateAttendance([FromBody] AttendanceUM attendanceUM)
        {
            var response = await _attendanceService.UpdateAttendance(attendanceUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        [Route("get-daily-attendance")]
        public async Task<IActionResult> GetDailyAttendance([FromQuery] int branchId, [FromQuery] DateTime date)
        {
            var response = await _attendanceService.GetDailyAttendance(branchId, date);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
