using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/nurse")]
	[Authorize(Roles = "Nurse")]
	public class NurseController : ControllerBase
	{
		private readonly INurseService _nurseService;
		private readonly IVaccinationRecordService _vaccinationRecordService;
		private readonly IHealthCheckupService _healthCheckupService;
		private readonly IConselingService _conselingService;

		public NurseController(INurseService nurseService, IVaccinationRecordService vaccinationRecordService, IHealthCheckupService healthCheckupService, IConselingService conselingService)
		{
			_nurseService = nurseService;
			_vaccinationRecordService = vaccinationRecordService;
			_healthCheckupService = healthCheckupService;
			_conselingService = conselingService;
		}

		[HttpGet("health-profiles/{studentId}")]
		public async Task<IActionResult> GetHealthProfile(string studentId)
		{
			var healthProfile = await _nurseService.GetHealthProfileByStudentIdAsync(studentId);
			if (healthProfile == null) return NotFound("Health profile not found.");
			return Ok(healthProfile);
		}

		[HttpPost("health-profiles")]
		public async Task<IActionResult> CreateHealthProfile(string studentId, [FromBody] HealthProfileRequest request)
		{
			var result = await _nurseService.CreateHealthProfileAsync(studentId, request);
			if (!result) return BadRequest("Failed to create health profile or student not found.");
			return Ok("Health profile created successfully.");
		}

		[HttpPost("health-profiles/import")]
		public async Task<IActionResult> ImportHealthProfiles(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No file uploaded.");
			}

			using (var stream = file.OpenReadStream())
			{
				await _nurseService.ImportHealthProfilesFromExcelAsync(stream);
			}

			return Ok("Health profiles imported successfully.");
		}


		[HttpPut("health-profiles/{studentId}")]
		public async Task<IActionResult> UpdateHealthProfile(string studentId, [FromBody] HealthProfileRequest request)
		{
			var result = await _nurseService.UpdateHealthProfileAsync(studentId, request);
			if (!result) return NotFound("Health profile not found.");
			return NoContent();
		}


		[HttpDelete("health-profiles/{studentId}")]
		public async Task<IActionResult> DeleteHealthProfile(string studentId)
		{
			var result = await _nurseService.DeleteHealthProfileAsync(studentId);
			if (!result) return NotFound("Health profile not found.");
			return NoContent();
		}

		[HttpGet("get-all-checkup")]
		public async Task<IActionResult> GetNurseCheckup()
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(nurseId))
			{
				return Unauthorized("Nurse ID not found in claims.");
			}
			var schedules = await _healthCheckupService.GetCheckingByNurse(nurseId);
			if (schedules == null || !schedules.Any()) return NotFound("No checkup found.");
			return Ok(schedules);
		}

		[HttpGet("health-checkup-records")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllHealthCheckupRecords()
		{
			var records = await _healthCheckupService.GetAllCheckupRecordsAsync();
			return Ok(records);
		}

		[HttpGet("health-checkup-records/by-date")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetCheckupRecordsByDate([FromQuery] DateTime date)
		{
			var records = await _healthCheckupService.GetCheckupRecordsByDateAsync(date);
			if (records == null || !records.Any())
			{
				return NotFound("No health checkup records found for the specified date.");
			}
			return Ok(records);
		}

		[HttpPut("health-checkup-records/{id}")]
		public async Task<IActionResult> UpdateHealthCheckupRecord(string id, [FromBody] HealthCheckupUpdateRequest request)
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(nurseId))
			{
				return Unauthorized("Nurse ID not found in claims.");
			}
			var result = await _healthCheckupService.UpdateCheckupRecordAsync(id, request, nurseId);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpGet("get-all-conseling-schedules")]
		public async Task<IActionResult> GetAllConselingSchedules()
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(nurseId))
			{
				return Unauthorized("Nurse ID not found in claims.");
			}
			var schedules = await _conselingService.GetSchedulesByNIdAsync(nurseId);
			if (schedules == null || !schedules.Any()) return NotFound("No counseling schedules found.");
			return Ok(schedules);
		}

		[HttpPut("accept-conseling-schedules")]
		public async Task<IActionResult> AcceptConselingSchedule([FromBody] AcceptConselingScheduleRequest request)
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(nurseId))
			{
				return Unauthorized("Nurse ID not found in claims.");
			}
			var result = await _conselingService.AcceptConselingScheduleAsync(request.ConselingScheduleId, request.ScheduledTime, nurseId);
			if (!result) return BadRequest("Failed to accept counseling schedule. Schedule not found or nurse ID mismatch.");
			return Ok(true);
		}

		[HttpGet("vaccination-records")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllVaccinationRecords()
		{
			var records = await _vaccinationRecordService.GetAllVaccinationRecordsAsync();
			return Ok(records);
		}

		[HttpPut("vaccination-records/{id}")]
		public async Task<IActionResult> UpdateVaccinationRecord(string id, [FromBody] VaccinationRecordRequest req)
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(nurseId))
			{
				return Unauthorized("Nurse ID not found in claims.");
			}
			var result = await _vaccinationRecordService.UpdateVaccinationRecordAsync(id, req, nurseId);
			if (!result) return NotFound();
			return NoContent();
		}
		[HttpGet("vaccination-records/by-date")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetVaccineRecordsByDate([FromQuery] DateTime date)
		{
			var records = await _vaccinationRecordService.GetVaccineRecordsByDateAsync(date);
			if (records == null || !records.Any())
			{
				return NotFound("No health checkup records found for the specified date.");
			}
			return Ok(records);
		}
	}
}
