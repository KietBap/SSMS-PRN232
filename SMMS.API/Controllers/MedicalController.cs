using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using System.Security.Claims;

namespace SMMS.API.Controllers
{
    [ApiController]
    [Route("api/medical")]
    public class MedicalController : ControllerBase
    {
        private readonly IMedicalService _medicalService;

        public MedicalController(IMedicalService medicalService)
        {
            _medicalService = medicalService;
        }

        //-----------------Medical Stock-----------------

        [HttpPost("stock")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> CreateMedicalStock([FromBody] CreateMedicalStockRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.CreateMedicalStockAsync(userId, request);
            if (!result)
                return BadRequest("Failed to create medical stock.");

            return Ok("MedicalStock created successfully.");
        }

        [HttpDelete("stock/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> DeleteMedicalStock(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.DeleteMedicalStockAsync(id, userId);
            if (!result) return BadRequest("Failed to delete medical stock.");
            return Ok("MedicalStock deleted successfully.");
        }

        [HttpGet("stock/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetMedicalStockById(string id)
        {
            var result = await _medicalService.GetMedicalStockByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("stock")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetAllMedicalStock()
        {
            var result = await _medicalService.GetAllMedicalStockAsync();

            return Ok(result);
        }

        [HttpPut("stock/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalStock(string id, UpdateMedicalStockRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateMedicalStockAsync(id, request, userId);
            if (!result) return BadRequest("Failed to update medical stock.");
            return Ok("MedicalStock updated successfully.");
        }


        //---------------Medical Incident----------------

        [HttpPost("incident")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> CreateMedicalIncident([FromBody] CreateMedicalIncidentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.CreateMedicalIncidentAsync(userId, request);
            if (!result)
                return BadRequest("Failed to create medical incident.");

            return Ok("MedicalIncident created successfully.");
        }

        [HttpDelete("incident/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> DeleteMedicalIncident(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.DeleteMedicalIncidentAsync(id, userId);
            if (!result) return BadRequest("Failed to delete medical incident.");
            return Ok("MedicalIncident deleted successfully.");
        }

        [HttpGet("incident/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse,Parent")]
        public async Task<IActionResult> GetMedicalIncidentById(string id)
        {
            var result = await _medicalService.GetMedicalIncidentByIdAsync(id);

            return Ok(result);
        }

        [HttpGet("incident")]
        [Authorize(Roles = "Admin,Manager,Nurse,Parent")]
        public async Task<IActionResult> GetAllMedicalIncident(string? studentId)
        {
            var result = await _medicalService.GetAllMedicalIncidentAsync(studentId);
            return Ok(result);
        }


        [HttpPut("incident/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalIncident(string id, UpdateMedicalIncidentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateMedicalIncidentAsync(id, request, userId);
            if (!result) return BadRequest("Failed to update medical incident.");
            return Ok("MedicalIncident updated successfully.");
        }


        //---------------Medical Usage----------------

        [HttpPost("usage")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> CreateMedicalUsage([FromBody] CreateMedicalUsageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.CreateMedicalUsageAsync(userId, request);
            if (!result)
                return BadRequest("Failed to create medical usage.");

            return Ok("MedicalUsage created successfully.");
        }

        [HttpDelete("usage/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> DeleteMedicalUsage(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.DeleteMedicalUsageAsync(id, userId);
            if (!result) return BadRequest("Failed to delete medical usage.");
            return Ok("MedicalUsage deleted successfully.");
        }

        [HttpPut("usage/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalUsage(string id, UpdateMedicalUsageRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateMedicalUsageAsync(id, request, userId);
            if (!result) return BadRequest("Failed to update medical usage.");
            return Ok("MedicalUsage updated successfully.");
        }


        //---------------Medical Request----------------

        [HttpPost("request")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> CreateMedicalRequest([FromBody] CreateMedicalRequestRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.CreateMedicalRequestAsync(userId, request);
            if (!result)
                return BadRequest("Failed to create medical request.");

            return Ok("Medical request created successfully.");
        }

        [HttpGet("request")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetAllMedicalRequests()
        {
            var result = await _medicalService.GetAllMedicalRequestsAsync();
            return Ok(result);
        }

        [HttpGet("request/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetMedicalRequestById(string id)
        {
            var result = await _medicalService.GetMedicalRequestByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("request/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalRequest(string id, UpdateMedicalRequestRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateMedicalRequestAsync(id, request, userId);
            if (!result) return BadRequest("Failed to update medical request.");
            return Ok("Medical request updated successfully.");
        }

        [HttpDelete("request/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> DeleteMedicalRequest(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.DeleteMedicalRequestAsync(id, userId);
            if (!result) return BadRequest("Failed to delete medical request.");
            return Ok("Medical request deleted successfully.");
        }

        [HttpGet("request/daily/{date}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetDailyMedicalRequests(DateTime date)
        {
            var result = await _medicalService.GetDailyMedicalRequestsAsync(date);
            return Ok(result);
        }

        [HttpGet("request/daily/today")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetTodayMedicalRequests()
        {
            var result = await _medicalService.GetDailyMedicalRequestsAsync(DateTime.Today);
            return Ok(result);
        }

        [HttpPut("request/{id}/complete")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> CompleteMedicalRequest(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.CompleteMedicalRequestAsync(id, userId);
            if (!result) return BadRequest("Failed to complete medical request.");
            return Ok("Medical request completed successfully.");
        }

        [HttpPut("request/{id}/status")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalRequestStatus(string id, UpdateMedicalRequestStatusRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateMedicalRequestStatusAsync(id, request.Status, userId);
            if (!result) return BadRequest("Failed to update medical request status.");
            return Ok("Medical request status updated successfully.");
        }

        [HttpGet("request/student/{studentId}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetMedicalRequestsByStudent(string studentId)
        {
            var result = await _medicalService.GetMedicalRequestsByStudentAsync(studentId);
            return Ok(result);
        }

        [HttpGet("request/status/{status}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetMedicalRequestsByStatus(string status)
        {
            var result = await _medicalService.GetMedicalRequestsByStatusAsync(status);
            return Ok(result);
        }

        [HttpGet("request/search")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> SearchMedicalRequests([FromQuery] string? medicalName, [FromQuery] string? studentId, [FromQuery] DateTime? date, [FromQuery] string? status)
        {
            var result = await _medicalService.SearchMedicalRequestsAsync(medicalName, studentId, date, status);
            return Ok(result);
        }

        [HttpPost("request/reset-daily-completion")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> ResetDailyCompletionStatus()
        {
            var result = await _medicalService.ResetDailyCompletionStatusAsync();
            if (!result) return BadRequest("Failed to reset daily completion status.");
            return Ok("Daily completion status reset successfully.");
        }

        [HttpGet("request/completion-status/{date}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetCompletionStatusByDate(DateTime date)
        {
            var result = await _medicalService.GetCompletionStatusByDateAsync(date);
            return Ok(result);
        }

        [HttpGet("request/completion-status/today")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetTodayCompletionStatus()
        {
            var result = await _medicalService.GetCompletionStatusByDateAsync(DateTime.Today);
            return Ok(result);
        }
    }
}
