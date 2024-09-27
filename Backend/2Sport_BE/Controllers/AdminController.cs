using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.Service.Services;
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
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public AdminController(
            IUserService userService,
            IEmployeeService employeeService,
            IMapper mapper
            )
        {
            _userService = userService;
            _employeeService = employeeService;
            _mapper = mapper;
        }
        //Role Admin
        [HttpGet]
        [Route("get-all-users")]
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
        [Route("get-all-employees")]
        public async Task<IActionResult> GetAllEmployee()
        {
            var response = await _employeeService.GetAllEmployees();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-user-detail")]
        public async Task<IActionResult> GetUserDetail([FromQuery] int id)
        {
            try
            {
                var response = await _userService.GetUserDetailsById(id);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpGet]
        [Route("get-employee-detail")]
        //Role User
        public async Task<IActionResult> GetEmployeeDetail([FromQuery] int id)
        {
            var response = await _employeeService.GetEmployeeDetailsById(id);
            if (response.IsSuccess)
            {
                return Ok(response);

            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-employees-by-branch")]
        public async Task<IActionResult> GetEmployeeByBranch([FromQuery] int branchId)
        {
            var response = await _employeeService.GetEmployeesByBranch(branchId);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
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
        [HttpPost]
        [Route("create-employee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCM employeeCM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _employeeService.AddEmployeeAsync(employeeCM);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpPut]
        [Route("change-status-user")]
        //Role Admin
        public async Task<ActionResult<User>> ChangeStatusUser([FromQuery] int id)
        {
            var response = await _userService.DisableUserAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut]
        [Route("change-status-employee")]
        //Role Admin
        public async Task<ActionResult<User>> ChangeStatusEmployee([FromQuery] int id)
        {
            var response = await _employeeService.DisableEmployeeAsync(id);
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
        [Route("update-employee")]
        //Role Admin
        public async Task<IActionResult> UpdateEmployeeAsync([FromQuery] int id, [FromBody] EmployeeUM employeeUM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _employeeService.UpdateEmployeeAsync(id, employeeUM);
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
        [HttpDelete]
        [Route("delete-employee")]
        //Role Admin
        public async Task<ActionResult<User>> DeleteEmployee([FromQuery] int id)
        {
            var response = await _employeeService.RemoveEmployeeAsync(id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-users-by-role")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            try
            {
                var query = await _userService.GetUserWithConditionAsync(_ => _.RoleId == roleId);
                var result = _mapper.Map<List<User>, List<UserVM>>(query.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
