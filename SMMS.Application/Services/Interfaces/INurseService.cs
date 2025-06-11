

using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface INurseService
	{
		Task<bool> CreateHealthProfileAsync(string studentId, HealthProfileRequest request);
		Task<HealthProfileResponse> GetHealthProfileByStudentIdAsync(string studentId);
		Task<bool> UpdateHealthProfileAsync(string studentId, HealthProfileRequest request);
		Task<bool> DeleteHealthProfileAsync(string studentId);
		Task ImportHealthProfilesFromExcelAsync(Stream fileStream);
	}
}
