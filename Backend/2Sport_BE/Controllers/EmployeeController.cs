using _2Sport_BE.DataContent;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public readonly IEmployeeService _employeeService;

        public EmployeeController(
            IEmployeeService employeeService,
            IUnitOfWork unitOfWork)
        {
            _employeeService = employeeService;
        }
        [Route("log-in")]
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

            var result = await _employeeService.LoginEmployeeAsync(loginModel);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [Route("log-out")]
        [HttpPost]
        public async Task<IActionResult> LogOutAsync([FromBody] TokenModel request)
        {
            var result = await _employeeService.LogoutEmployeeAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("get-profile")]
        public async Task<IActionResult> GetEmployeeDetail([FromQuery] int id)
        {
            try
            {
                var result = await _employeeService.GetEmployeeDetailsById(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [Route("create-admin-account")]
        [HttpPost]
        public async Task<IActionResult> CreateAdminAccount([FromBody] EmployeeCM employeeCM)
        {
            var result = await _employeeService.CreateANewAdminAccount(employeeCM);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPut]
        [Route("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromQuery] int id, [FromBody] ChangePasswordVM changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdatePasswordAsync(id, changePasswordVM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        [Route("update-employee")]
        public async Task<IActionResult> UpdateEmployeeAsync([FromQuery] int employeeId, [FromBody] EmployeeUM employeeUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _employeeService.UpdateEmployeeAsync(employeeId, employeeUM);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
