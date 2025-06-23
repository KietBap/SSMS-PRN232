
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Domain.Enum;

namespace SMMS.Application.Services.Interfaces
{
	public interface IConselingService
	{
		Task<bool> RequestConselingScheduleAsync(string studentId, string healthCheckupId, DateTime requestedDate, string note);
		Task<bool> UpdateScheduleStatusAsync(string conselingScheduleId, ApprovalStatus status, string parentId);
		Task<List<ConselingResponse>> GetSchedulesByNIdAsync(string nurseId);
		Task<List<ConselingResponse>> GetSchedulesByPIdAsync(string parentId);
	}
}
