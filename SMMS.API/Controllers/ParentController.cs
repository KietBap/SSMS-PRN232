using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;

using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Enum;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/parents")]
	[Authorize(Roles = "Parent")]
	public class ParentController : ControllerBase
	{
		private readonly IConselingService _conselingService;
		private readonly IUserService _userService;
		private readonly IHealthCheckupService _healthCheckupService;
		private readonly IActivityConsentService _consentService;

		public ParentController(IConselingService conselingService, IUserService userService, IHealthCheckupService healthCheckupService, IActivityConsentService consentService)
		{
			_conselingService = conselingService;
			_userService = userService;
			_healthCheckupService = healthCheckupService;
			_consentService = consentService;
		}

		[HttpGet("students")]
		[Authorize(Roles = "Parent")]
		public async Task<IActionResult> GetMyStudents()
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized();
			var students = await _userService.GetMyStudentsAsync(parentId);
			return Ok(students);
		}

		[HttpGet("get-all-student-health-checkup")]
		public async Task<IActionResult> GetHealthCheckup()
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized("Parent ID not found.");
			var schedules = await _healthCheckupService.GetCheckingByParent(parentId);
			if (schedules == null || !schedules.Any()) return NotFound("No checkup found.");
			return Ok(schedules);
		}


		[HttpGet("get-all-conseling-schedules")]
		public async Task<IActionResult> GetAllConselingSchedules()
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized("Parent ID not found.");
			var schedules = await _conselingService.GetSchedulesByPIdAsync(parentId);
			if (schedules == null || !schedules.Any()) return NotFound("No counseling schedules found.");
			return Ok(schedules);
		}

		[HttpPost("conseling-schedules")]
		public async Task<IActionResult> RequestConselingSchedule([FromBody] ConselingRequest request)
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized("Parent ID not found.");
			var result = await _conselingService.RequestConselingScheduleAsync(request.StudentId, request.HealthCheckupId, request.RequestedDate, parentId, request.Note);
			if (!result) return BadRequest("Failed to request schedule.");
			return Ok("Schedule requested.");
		}

		[HttpPut("students/{studentId}/health-profile")]
		public async Task<IActionResult> UpdateStudentHealthProfile(string studentId, [FromBody] HealthProfileRequest request)
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized();
			var result = await _userService.UpdateHealthProfileByParentAsync(studentId, request, parentId);
			if (!result) return BadRequest("Không thể cập nhật hồ sơ sức khỏe. Học sinh không tồn tại hoặc không thuộc về phụ huynh này.");
			return NoContent();
		}

		[HttpGet("activity-consents/my-children")]
		public async Task<IActionResult> GetActivityConsentsForMyChildren()
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized("Parent ID not found.");
			var consents = await _consentService.GetConsentsByParentIdAsync(parentId);
			return Ok(consents);
		}


		[HttpPut("activity-consents/{id}/status")]
		[Authorize(Roles = "Parent")]
		public async Task<IActionResult> UpdateActivityConsentStatus(string id, [FromBody] ApprovalStatus status)
		{
			var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(parentId)) return Unauthorized("Parent ID not found.");
			if (status != ApprovalStatus.Approved && status != ApprovalStatus.Rejected)
				return BadRequest("Invalid status. Only Approved or Rejected are allowed.");
			var result = await _consentService.UpdateActivityConsentStatusAsync(id, status, parentId);
			if (!result) return BadRequest("Failed to update consent status.");
			return NoContent();
		}
	}
}
