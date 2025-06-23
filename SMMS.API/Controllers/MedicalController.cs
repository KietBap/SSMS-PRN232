using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;
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

        [HttpPatch("incident/status/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateIncidentStatus(string id, MedicalIncidentStatus status)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.UpdateIncidentStatusAsync(id, status, userId);
            return Ok(result);
        }


        //---------------Medical Usage----------------

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
        [Authorize(Roles = "Admin,Manager,Nurse,Parent")]
        public async Task<IActionResult> GetMedicalRequestById(string id)
        {
            var result = await _medicalService.GetMedicalRequestByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("request/{id}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> UpdateMedicalRequest(string id, [FromBody] UpdateMedicalRequestRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated.");
                }

                var result = await _medicalService.UpdateMedicalRequestAsync(id, request, userId);
                return Ok(new {
                    success = true,
                    message = "Medical request updated successfully.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the medical request." });
            }
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

        [HttpGet("request/student/{studentId}")]
        [Authorize(Roles = "Admin,Manager,Nurse,Parent")]
        public async Task<IActionResult> GetMedicalRequestsByStudent(string studentId)
        {
            var result = await _medicalService.GetMedicalRequestsByStudentAsync(studentId);
            return Ok(result);
        }

        [HttpGet("request/parent/{parentId}")]
        [Authorize(Roles = "Admin,Manager,Nurse,Parent")]
        public async Task<IActionResult> GetMedicalRequestsByParent(string parentId)
        {
            var result = await _medicalService.GetMedicalRequestsByParentAsync(parentId);
            return Ok(result);
        }

        [HttpGet("request/daily/{date}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetDailyMedicationSchedule(DateTime date)
        {
            var result = await _medicalService.GetDailyMedicationScheduleAsync(date);
            return Ok(result);
        }

        [HttpGet("request/daily/today")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetTodayMedicationSchedule()
        {
            var result = await _medicalService.GetDailyMedicationScheduleAsync(DateTime.Today);
            return Ok(result);
        }

        [HttpPost("request/administration")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> RecordMedicationAdministration([FromBody] RecordMedicationAdministrationRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _medicalService.RecordMedicationAdministrationAsync(userId, request);
            if (!result) return BadRequest("Failed to record medication administration.");
            return Ok("Medication administration recorded successfully.");
        }

        [HttpGet("request/history/completed/{date}")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetCompletedMedicationHistory(DateTime date)
        {
            try
            {
                var result = await _medicalService.GetCompletedMedicationHistoryAsync(date);
                return Ok(new {
                    success = true,
                    message = "Completed medication history retrieved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving completed medication history." });
            }
        }

        [HttpGet("request/history/completed/today")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> GetTodayCompletedMedicationHistory()
        {
            try
            {
                var result = await _medicalService.GetTodayCompletedMedicationHistoryAsync();
                return Ok(new {
                    success = true,
                    message = "Today's completed medication history retrieved successfully.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving today's completed medication history." });
            }
        }

        [HttpGet("request/search")]
        [Authorize(Roles = "Admin,Manager,Nurse")]
        public async Task<IActionResult> SearchMedicalRequests(
            [FromQuery] string? medicationName,
            [FromQuery] string? studentId,
            [FromQuery] DateTime? date,
            [FromQuery] string? status)
        {
            var result = await _medicalService.SearchMedicalRequestsAsync(medicationName, studentId, date, status);
            return Ok(result);
        }

    }
}
