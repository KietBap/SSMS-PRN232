using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface ISchoolClassService
	{
		Task<List<SchoolClassResponse>> GetAllSchoolClassesAsync();
		Task<SchoolClassResponse> GetSchoolClassByIdAsync(string id);
		Task<bool> CreateSchoolClassAsync(SchoolClassRequest request);
		Task<bool> UpdateSchoolClassAsync(string id, SchoolClassRequest request);
		Task<bool> DeleteSchoolClassAsync(string id);
	}
}
