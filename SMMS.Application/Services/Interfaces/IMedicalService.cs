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


        //---------------Medical Usage----------------
        Task<bool> CreateMedicalUsageAsync(string userId, CreateMedicalUsageRequest request);
        Task<bool> DeleteMedicalUsageAsync(string id, string userId);
        Task<bool> UpdateMedicalUsageAsync(string id, UpdateMedicalUsageRequest model, string userId);


        //---------------Medical Request----------------
        Task<bool> CreateMedicalRequestAsync(string userId, CreateMedicalRequestRequest request);
        Task<List<ListMedicalRequestResponse>> GetAllMedicalRequestsAsync();
        Task<MedicalRequestResponse> GetMedicalRequestByIdAsync(string id);
        Task<bool> UpdateMedicalRequestAsync(string id, UpdateMedicalRequestRequest request, string userId);
        Task<bool> DeleteMedicalRequestAsync(string id, string userId);
        Task<List<DailyMedicalRequestResponse>> GetDailyMedicalRequestsAsync(DateTime date);
        Task<bool> CompleteMedicalRequestAsync(string id, string userId);
        Task<bool> UpdateMedicalRequestStatusAsync(string id, string status, string userId);
        Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByStudentAsync(string studentId);
        Task<List<ListMedicalRequestResponse>> GetMedicalRequestsByStatusAsync(string status);
        Task<List<ListMedicalRequestResponse>> SearchMedicalRequestsAsync(string? medicalName, string? studentId, DateTime? date, string? status);
        Task<bool> ResetDailyCompletionStatusAsync();
        Task<object> GetCompletionStatusByDateAsync(DateTime date);

    }
}
