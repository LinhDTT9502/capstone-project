using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2Sport_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet]
        [Route("get-all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var response = await _roleService.GetAllRoles();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet]
        [Route("get-role-detail")]
        public async Task<IActionResult> GetRoleDetail([FromQuery] int roleId)
        {
            var role = _roleService.GetRoleById(roleId);
            return Ok(role);
        }
        
    }
}
