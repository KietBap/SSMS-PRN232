using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
	[ApiController]
	[Route("api/medical-events")]
	public class MedicalEventController : ControllerBase
	{
		private readonly IHealthActivityService _healthActivityService;
		private readonly IVaccinationCampaignService _vaccinationCampaignService;
		private readonly IActivityConsentService _consentService;

		public MedicalEventController(IHealthActivityService healthActivityService, IVaccinationCampaignService vaccinationCampaignService, IActivityConsentService consentService)
		{
			_healthActivityService = healthActivityService;
			_vaccinationCampaignService = vaccinationCampaignService;
			_consentService = consentService;
		}

		[HttpGet("health-activities/all")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllHealthActivities()
		{
			var activities = await _healthActivityService.GetAllHealthActivityAsync();
			return Ok(activities);
		}

		[HttpGet("health-activities/approve-reject")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetHealthActivitiesWithoutPending()
		{
			var activities = await _healthActivityService.GetActivityWithoutPendingAsync();
			return Ok(activities);
		}

		[HttpGet("health-activities/pending")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetPendingHealthActivities()
		{
			var pending = await _healthActivityService.GetPendingHealthActivitiesAsync();
			return Ok(pending);
		}

		[HttpPost("health-activities")]
		[Authorize(Roles = "Nurse")]
		public async Task<IActionResult> CreateHealthActivity([FromBody] HealthActivityRequest request)
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (nurseId == null) return Unauthorized("Nurse ID not found in claims.");
			var response = await _healthActivityService.CreateHealthActivityAsync(request, nurseId);
			return Ok(response);
		}


		[HttpPut("health-activities/{id}/approve-or-reject")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> UpdateHealthActivityStatus(string id, [FromQuery] string action)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");

			if (action != "approve" && action != "reject") return BadRequest("Invalid action. Must be 'approve' or 'reject'.");

			var result = await _healthActivityService.UpdateHealthActivityStatusAsync(id, action, userId);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpPut("health-activities/{id}")]
		[Authorize(Roles = "Nurse,Admin,Manager")]
		public async Task<IActionResult> UpdateHealthActivity(string id, [FromBody] HealthActivityRequest request)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");
			var result = await _healthActivityService.UpdateHealthActivityAsync(id, request, userId);
			if (!result) return BadRequest("Cannot update health activity.");
			return NoContent();
		}
		[HttpDelete("health-activities/{id}")]
		[Authorize(Roles = "Nurse,Admin,Manager")]
		public async Task<IActionResult> DeleteHealthActivity(string id)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");
			var result = await _healthActivityService.DeleteHealthActivityAsync(id, userId);
			if (!result) return BadRequest("Cannot delete health activity.");
			return NoContent();
		}

		[HttpGet("vaccination-campaigns/all")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetAllVaccineCampaign()
		{
			var activities = await _vaccinationCampaignService.GetAllVaccineCampaignAsync();
			return Ok(activities);
		}

		[HttpGet("vaccination-campaigns/pending")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetPendingVaccinationCampaigns()
		{
			var pending = await _vaccinationCampaignService.GetPendingVaccinationCampaignsAsync();
			return Ok(pending);
		}
		[HttpGet("vaccination-campaigns/approve-reject")]
		[Authorize(Roles = "Admin,Manager,Nurse,Parent")]
		public async Task<IActionResult> GetApprovedVaccinationCampaigns()
		{
			var campaigns = await _vaccinationCampaignService.GetCampaignWithoutPendingAsync();
			return Ok(campaigns);
		}

		[HttpPost("vaccination-campaigns")]
		[Authorize(Roles = "Nurse")]
		public async Task<IActionResult> CreateVaccinationCampaign([FromBody] VaccinationCampaignRequest request)
		{
			var nurseId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (nurseId == null) return Unauthorized("Nurse ID not found in claims.");
			var response = await _vaccinationCampaignService.CreateVaccinationCampaignAsync(request, nurseId);
			return Ok(response);
		}

		[HttpPut("vaccination-campaigns/{id}/approve-or-reject")]
		[Authorize(Roles = "Admin,Manager")]
		public async Task<IActionResult> ApproveVaccinationCampaign(string id, [FromQuery] string action)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");
			if (action != "approve" && action != "reject") return BadRequest("Invalid action. Must be 'approve' or 'reject'.");
			var result = await _vaccinationCampaignService.UpdateVaccineCampaignStatusAsync(id, action, userId);
			if (!result) return NotFound();
			return NoContent();
		}

		[HttpPut("vaccination-campaigns/{id}")]
		[Authorize(Roles = "Nurse,Admin,Manager")]
		public async Task<IActionResult> UpdateVaccinationCampaign(string id, [FromBody] VaccinationCampaignRequest request)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");
			var result = await _vaccinationCampaignService.UpdateVaccinationCampaignAsync(id, request, userId);
			if (!result) return BadRequest("Cannot update vaccination campaign.");
			return NoContent();
		}

		[HttpDelete("vaccination-campaigns/{id}")]
		[Authorize(Roles = "Nurse,Admin,Manager")]
		public async Task<IActionResult> DeleteVaccinationCampaign(string id)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized("User ID not found in claims.");
			var result = await _vaccinationCampaignService.DeleteVaccinationCampaignAsync(id, userId);
			if (!result) return BadRequest("Cannot delete vaccination campaign.");
			return NoContent();
		}

		[HttpGet("activity-consents/health-activities/{healthActivityId}")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetActivityConsentsForHealthActivity(string healthActivityId)
		{
			var consents = await _consentService.GetConsentsByHAIdAsync(healthActivityId);
			return Ok(consents);
		}

		[HttpGet("activity-consents/vaccination-campaigns/{vaccinationCampaignId}")]
		[Authorize(Roles = "Admin,Manager,Nurse")]
		public async Task<IActionResult> GetActivityConsentsForVaccinationCampaign(string vaccinationCampaignId)
		{
			var consents = await _consentService.GetConsentsByVCIdAsync(vaccinationCampaignId);
			return Ok(consents);
		}

	}
}
