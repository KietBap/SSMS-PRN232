

using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface IVaccinationRecordService
	{
		Task<List<VaccinationRecordResponse>> GetVaccinationRecordsByParentIdAsync(string parentId);
		Task<bool> UpdateVaccinationRecordAsync(string vaccinationRecordId, VaccinationRecordRequest request, string nurseId);
		Task<List<VaccinationRecordResponse>> GetVaccinationRecordsByStudentIdAsync(string studentId);
		Task<List<VaccinationRecordResponse>> GetAllVaccinationRecordsAsync();
		Task<List<VaccinationRecordResponse>> GetVaccineRecordsByDateAndIdAsync(string campaignId, DateTime date);
	}
}
