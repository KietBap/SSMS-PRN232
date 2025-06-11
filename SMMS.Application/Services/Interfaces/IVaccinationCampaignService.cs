

using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface IVaccinationCampaignService
	{
		Task<List<VaccinationCampaignResponse>> GetAllVaccineCampaignAsync();
		Task<List<VaccinationCampaignResponse>> GetPendingVaccinationCampaignsAsync();
		Task<List<VaccinationCampaignResponse>> GetCampaignWithoutPendingAsync();
		Task<VaccinationCampaignResponse> CreateVaccinationCampaignAsync(VaccinationCampaignRequest request, string nurseId);
		Task<bool> UpdateVaccineCampaignStatusAsync(string vcId, string action, string userId);
		Task<bool> UpdateVaccinationCampaignAsync(string vaccinationCampaignId, VaccinationCampaignRequest request, string userId);
		Task<bool> DeleteVaccinationCampaignAsync(string vaccinationCampaignId, string userId);
	}
}
