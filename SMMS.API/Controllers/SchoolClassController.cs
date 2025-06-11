using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/school-classes")]
	public class SchoolClassController : ControllerBase
	{
		private readonly ISchoolClassService _schoolClassService;

		public SchoolClassController(ISchoolClassService schoolClassService)
		{
			_schoolClassService = schoolClassService;
		}

		[HttpGet]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllSchoolClasses()
		{
			var classes = await _schoolClassService.GetAllSchoolClassesAsync();
			return Ok(classes);
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetSchoolClassById(string id)
		{
			var schoolClass = await _schoolClassService.GetSchoolClassByIdAsync(id);
			if (schoolClass == null) return NotFound("School class not found.");
			return Ok(schoolClass);
		}

		[HttpPost]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> CreateSchoolClass([FromBody] SchoolClassRequest request)
		{
			var result = await _schoolClassService.CreateSchoolClassAsync(request);
			if (!result) return BadRequest("Failed to create school class.");
			return Ok("School class created successfully.");
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> UpdateSchoolClass(string id, [FromBody] SchoolClassRequest request)
		{
			var result = await _schoolClassService.UpdateSchoolClassAsync(id, request);
			if (!result) return NotFound("School class not found.");
			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> DeleteSchoolClass(string id)
		{
			var result = await _schoolClassService.DeleteSchoolClassAsync(id);
			if (!result) return NotFound("School class not found.");
			return NoContent();
		}
	}
}
