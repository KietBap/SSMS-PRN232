using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Domain.Entity;
namespace SMMS.Application.Services.Interfaces
{
	public interface IHealthCheckupService
	{
		Task<bool> UpdateCheckupRecordAsync(string healthCheckupRecordId, HealthCheckupUpdateRequest request, string nurseId);

		Task<List<HealthCheckUpResponse>> GetCheckingByParent(string parentId);

		Task<List<HealthCheckUpResponse>> GetCheckingByNurse(string nurseId);

		Task UpdateHealthProfileAsync(HealthCheckupRecord record);

		Task<List<HealthCheckUpResponse>> GetCheckupRecordsBySIdAsync(string studentId);
		Task<List<HealthCheckUpResponse>> GetAllCheckupRecordsAsync();
		Task<List<HealthCheckUpResponse>> GetCheckupRecordsByDateAsync(DateTime date);
	}
}
