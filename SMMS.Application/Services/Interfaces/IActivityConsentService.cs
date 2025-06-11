using SMMS.Application.DataObject.ResponseObject;
using SMMS.Domain.Enum;

namespace SMMS.Application.Services.Interfaces
{
	public interface IActivityConsentService
	{
		Task<bool> UpdateActivityConsentStatusAsync(string activityConsentId, ApprovalStatus status, string parentId);
		Task<List<ActivityConsentResponse>> GetConsentsByParentIdAsync(string parentId);
		Task<List<ActivityConsentResponse>> GetConsentsByHAIdAsync(string healthActivityId);
		Task<List<ActivityConsentResponse>> GetConsentsByVCIdAsync(string vaccinationCampaignId); 
	}
}
