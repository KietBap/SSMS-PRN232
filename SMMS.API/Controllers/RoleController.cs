using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/roles")]
	[Authorize(Roles = "Admin")]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;

		public RoleController(IRoleService roleService)
		{
			_roleService = roleService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllRoles()
		{
			var roles = await _roleService.GetAllRolesAsync();
			return Ok(roles);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetRoleById(string id)
		{
			var role = await _roleService.GetRoleByIdAsync(id);
			if (role == null) return NotFound("Role not found.");
			return Ok(role);
		}

		[HttpPost]
		public async Task<IActionResult> CreateRole([FromBody] RoleRequest request)
		{
			var result = await _roleService.CreateRoleAsync(request);
			if (!result) return BadRequest("Failed to create role.");
			return Ok("Role created successfully.");
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleRequest request)
		{
			var result = await _roleService.UpdateRoleAsync(id, request);
			if (!result) return NotFound("Role not found.");
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var result = await _roleService.DeleteRoleAsync(id);
			if (!result) return NotFound("Role not found.");
			return NoContent();
		}
	}
}
