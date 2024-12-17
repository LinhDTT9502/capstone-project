using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2Sport_BE.Infrastructure.DTOs;

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
        [Route("list-all-roles")]
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
        [Route("get-role-details/{roleId}")]
        public async Task<IActionResult> GetRoleDetail(int roleId)
        {
            var response = await _roleService.GetRoleDetails(roleId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [HttpPost]
        [Route("add-role")]
        public async Task<IActionResult> AddRole([FromBody] RoleCM roleCM)
        {
            var response = await _roleService.CreateRole(roleCM);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [HttpPut]
        [Route("edit-role/{roleId}")]
        public async Task<IActionResult> EditRole(int roleId, [FromBody] RoleUM roleUM)
        {
            var response = await _roleService.UpdateRole(roleId, roleUM);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
        [HttpDelete]
        [Route("remove-role")]
        public async Task<IActionResult> RemoveRole([FromQuery] int roleId)
        {
            var response = await _roleService.DeleteRole(roleId);
            if (response.IsSuccess) return Ok(response);
            return BadRequest(response);
        }
    }
}
