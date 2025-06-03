using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

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


        //---------------Medical Usage----------------
        Task<bool> CreateMedicalUsageAsync(string userId, CreateMedicalUsageRequest request);
        Task<bool> DeleteMedicalUsageAsync(string id, string userId);
        Task<bool> UpdateMedicalUsageAsync(string id, UpdateMedicalUsageRequest model, string userId);

    }
}
