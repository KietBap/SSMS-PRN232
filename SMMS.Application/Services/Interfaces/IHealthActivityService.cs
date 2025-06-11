using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
namespace SMMS.Application.Services.Interfaces
{
	public interface IHealthActivityService
	{
		Task<HealthActivityResponse> CreateHealthActivityAsync(HealthActivityRequest request, string nurseId);

		Task<List<HealthActivityResponse>> GetActivityWithoutPendingAsync();
		Task<List<HealthActivityResponse>> GetPendingHealthActivitiesAsync();
		Task<List<HealthActivityResponse>> GetAllHealthActivityAsync();
		Task<bool> UpdateHealthActivityStatusAsync(string healthActivityId, string action, string userId);
		Task<bool> UpdateHealthActivityAsync(string healthActivityId, HealthActivityRequest request, string userId);
		Task<bool> DeleteHealthActivityAsync(string healthActivityId, string userId);

	}
}
