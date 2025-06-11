
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface IConselingService
	{
		Task<bool> RequestConselingScheduleAsync(string studentId, string healthCheckupId, DateTime requestedDate, string parentId, string note);
		Task<bool> AcceptConselingScheduleAsync(string conselingScheduleId, DateTime scheduledTime, string nurseId);
		Task<List<ConselingResponse>> GetSchedulesByNIdAsync(string nurseId);
		Task<List<ConselingResponse>> GetSchedulesByPIdAsync(string parentId);
	}
}
