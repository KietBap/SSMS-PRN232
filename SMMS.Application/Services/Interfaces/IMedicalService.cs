using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Domain.Enum;

namespace SMMS.Application.Services.Interfaces
{
    public interface IMedicalService
    {
        //-----------------Medical Stock-----------------
        Task<bool> CreateMedicalStockAsync(string userId, CreateMedicalStockRequest request);
        Task<bool> DeleteMedicalStockAsync(string id, string userId);
        Task<MedicalStockResponse> GetMedicalStockByIdAsync(string id);
        Task<List<ListMedicalStockResponse>> GetAllMedicalStockAsync();
        Task<bool> UpdateMedicalStockAsync(string id, UpdateMedicalStockRequest model, string userId);


        //---------------Medical Incident----------------
        Task<bool> CreateMedicalIncidentAsync(string userId, CreateMedicalIncidentRequest request);
        Task<bool> DeleteMedicalIncidentAsync(string id, string userId);
        Task<MedicalIncidentResponse> GetMedicalIncidentByIdAsync(string id);
        Task<List<ListMedicalIncidentResponse>> GetAllMedicalIncidentAsync(string? studentId = null);
        Task<bool> UpdateMedicalIncidentAsync(string id, UpdateMedicalIncidentRequest model, string userId);
        Task<bool> UpdateIncidentStatusAsync(string id, MedicalIncidentStatus status, string userId);


        //---------------Medical Request----------------
        Task<bool> CreateMedicalRequestAsync(string userId, CreateMedicalRequestRequest request);
        Task<List<ListMedicalRequestResponse>> GetAllMedicalRequestsAsync();
        Task<MedicalRequestResponse> GetMedicalRequestByIdAsync(string id);
        Task<MedicalRequestResponse> UpdateMedicalRequestAsync(string id, UpdateMedicalRequestRequest request, string userId);
        Task<bool> DeleteMedicalRequestAsync(string id, string userId);
        Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByStudentAsync(string studentId);
        Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByParentAsync(string parentId);
        Task<List<DailyMedicationScheduleResponse>> GetDailyMedicationScheduleAsync(DateTime date);
        Task<bool> RecordMedicationAdministrationAsync(string userId, RecordMedicationAdministrationRequest request);
        Task<DailyCompletedMedicationSummary> GetCompletedMedicationHistoryAsync(DateTime date);
        Task<DailyCompletedMedicationSummary> GetTodayCompletedMedicationHistoryAsync();
        Task<List<ListMedicalRequestResponse>> SearchMedicalRequestsAsync(string? medicationName, string? studentId, DateTime? date, string? status);


        //---------------Medical Usage----------------
        Task<bool> DeleteMedicalUsageAsync(string id, string userId);
        Task<bool> UpdateMedicalUsageAsync(string id, UpdateMedicalUsageRequest model, string userId);

    }
}
