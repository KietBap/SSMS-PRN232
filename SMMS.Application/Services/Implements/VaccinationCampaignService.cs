

using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class VaccinationCampaignService : IVaccinationCampaignService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly INotificationService _notificationService;

		public VaccinationCampaignService(IRepositoryManager repositoryManager, INotificationService notificationService)
		{
			_repositoryManager = repositoryManager;
			_notificationService = notificationService;
		}

		public async Task<VaccinationCampaignResponse> CreateVaccinationCampaignAsync(VaccinationCampaignRequest request, string nurseId)
		{
			var existingClassIds = _repositoryManager.ClassRepository
			.FindByCondition(c => request.ClassIds.Contains(c.Id) && c.DeletedTime == null, false)
			.Select(c => c.Id)
			.ToList();
			if (existingClassIds.Count != request.ClassIds.Count)
			{
				throw new Exception("Một hoặc nhiều lớp không tồn tại.");
			}
			var campaign = new VaccinationCampaign
			{
				UserId = nurseId,
				Name = request.Name ?? string.Empty,
				VaccineName = request.VaccineName ?? string.Empty,
				EXP = request.EXP,
				MFG = request.MFG,
				VaccineType = request.VaccineType ?? string.Empty,
				StartDate = request.StartDate,
				Status = ApprovalStatus.Pending,
				CreatedBy = nurseId,
				CreatedTime = DateTimeOffset.UtcNow,
				VaccinationCampaignClasses = request.ClassIds.Select(classId => new VaccinationCampaignClass
				{
					SchoolClassId = classId ?? string.Empty,
					CreatedBy = nurseId,
					CreatedTime = DateTimeOffset.UtcNow
				}).ToList()
			};
			_repositoryManager.VaccinationCampaignRepository.Create(campaign);
			await _repositoryManager.SaveAsync();

			// Notify Admins///////////////////////////////////////////
			var admins = await _repositoryManager.UserRepository
				.FindByCondition(u => u.Role.RoleName == "Admin" && u.DeletedTime == null, false)
				.ToListAsync();
			foreach (var admin in admins)
			{
				await _notificationService.CreateNotificationAsync(
					admin.Id,
					"New Vaccination Campaign Needs Approval",
					$"Campaign: {campaign.Name} created by Nurse requires your approval."
					, campaign.Id
				);
			}

			return new VaccinationCampaignResponse
			{
				Id = campaign.Id,
				Name = campaign.Name,
				VaccineName = campaign.VaccineName,
				NurseId = campaign.UserId,
				NurseName = _repositoryManager.UserRepository
					.FindByCondition(u => u.Id == campaign.UserId, false)
					.Select(u => u.FullName)
					.FirstOrDefault(),
				EXP = campaign.EXP,
				MFG = campaign.MFG,
				VaccineType = campaign.VaccineType,
				StartDate = campaign.StartDate,
				Status = campaign.Status,
				ClassIds = campaign.VaccinationCampaignClasses.Select(vcc => vcc.SchoolClassId).ToList()
			};
		}

		public async Task<bool> UpdateVaccineCampaignStatusAsync(string vcId, string action, string userId)
		{
			var vaccination = _repositoryManager.VaccinationCampaignRepository
				.FindByCondition(vc => vc.Id == vcId && vc.Status == ApprovalStatus.Pending && vc.DeletedTime == null, true)
				.Include(vc => vc.VaccinationCampaignClasses)
				.FirstOrDefault();
			if (vaccination == null) return false;

			if (action == "approve")
			{
				vaccination.Status = ApprovalStatus.Approved;
				await CreateActivityConsentsAsync(vaccination);

				// Notify Nurse///////////////////////
				await _notificationService.CreateNotificationAsync(
					vaccination.UserId,
					"Vaccination Campaign Approved",
					$"Your campaign: {vaccination.Name} has been approved."
					, vaccination.Id
				);
			}
			else if (action == "reject")
			{
				vaccination.Status = ApprovalStatus.Rejected;
			}
			else
			{
				return false;
			}

			vaccination.LastUpdatedBy = userId;
			vaccination.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.VaccinationCampaignRepository.Update(vaccination);
			await _repositoryManager.SaveAsync();
			return true;
		}

		private async Task CreateActivityConsentsAsync(VaccinationCampaign campaign)
		{
			var classIds = campaign.VaccinationCampaignClasses.Select(vcc => vcc.SchoolClassId).ToList();
			var students = await Task.Run(() => _repositoryManager.StudentRepository
				.FindByCondition(s => classIds.Contains(s.ClassId) && s.DeletedTime == null, false)
				.ToList());

			foreach (var student in students)
			{
				var consent = new ActivityConsent
				{
					StudentId = student.Id,
					UserId = student.ParentId,
					ScheduleTime = campaign.StartDate,
					VaccinationCampaignId = campaign.Id,
					HealthActivityId = null,
					Status = ApprovalStatus.Pending,
					Comments = campaign.VaccineName,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow,
					ActivityType = "VaccinationCampaign"
				};
				_repositoryManager.ConsentRepository.Create(consent);
				await _repositoryManager.SaveAsync();

				// Create notification for each consent/////////////////////////////////////
				await _notificationService.CreateNotificationAsync(
						student.ParentId,
						"New Vaccination Campaign for Your Child",
						$"Campaign: {campaign.Name}. Please confirm participation."
						, campaign.Id
					);
			}
		}

		public async Task<List<VaccinationCampaignResponse>> GetAllVaccineCampaignAsync()
		{
			return await Task.Run(() => _repositoryManager.VaccinationCampaignRepository
				.FindAll(false)
				.Include(vc => vc.VaccinationCampaignClasses)
				.Include(vc => vc.User)
				.Select(vc => new VaccinationCampaignResponse
				{
					Id = vc.Id,
					Name = vc.Name,
					VaccineName = vc.VaccineName,
					NurseId = vc.UserId,
					NurseName = vc.User.FullName,
					EXP = vc.EXP,
					MFG = vc.MFG,
					VaccineType = vc.VaccineType,
					StartDate = vc.StartDate,
					Status = vc.Status,
					ClassIds = vc.VaccinationCampaignClasses.Select(vcc => vcc.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<List<VaccinationCampaignResponse>> GetPendingVaccinationCampaignsAsync()
		{
			return await Task.Run(() => _repositoryManager.VaccinationCampaignRepository
				.FindByCondition(vc => vc.Status == ApprovalStatus.Pending, false)
				.Include(vc => vc.VaccinationCampaignClasses)
				.Include(vc => vc.User)
				.Select(vc => new VaccinationCampaignResponse
				{
					Id = vc.Id,
					Name = vc.Name,
					VaccineName = vc.VaccineName,
					NurseId = vc.UserId,
					NurseName = vc.User.FullName,
					EXP = vc.EXP,
					MFG = vc.MFG,
					VaccineType = vc.VaccineType,
					StartDate = vc.StartDate,
					Status = vc.Status,
					ClassIds = vc.VaccinationCampaignClasses.Select(vcc => vcc.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<List<VaccinationCampaignResponse>> GetCampaignWithoutPendingAsync()
		{
			return await Task.Run(() => _repositoryManager.VaccinationCampaignRepository
				.FindByCondition(vc => vc.Status == ApprovalStatus.Approved || vc.Status == ApprovalStatus.Rejected, false)
				.Include(vc => vc.VaccinationCampaignClasses)
				.Include(vc => vc.User)
				.Select(vc => new VaccinationCampaignResponse
				{
					Id = vc.Id,
					Name = vc.Name,
					VaccineName = vc.VaccineName,
					NurseId = vc.UserId,
					NurseName = vc.User.FullName,
					EXP = vc.EXP,
					MFG = vc.MFG,
					VaccineType = vc.VaccineType,
					StartDate = vc.StartDate,
					Status = vc.Status,
					ClassIds = vc.VaccinationCampaignClasses.Select(vcc => vcc.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<bool> UpdateVaccinationCampaignAsync(string vaccinationCampaignId, VaccinationCampaignRequest request, string userId)
		{
			var campaign = _repositoryManager.VaccinationCampaignRepository
				.FindByCondition(vc => vc.Id == vaccinationCampaignId && vc.Status == ApprovalStatus.Pending, true)
				.FirstOrDefault();
			if (campaign == null) return false;

			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false).FirstOrDefault();
			if (campaign.CreatedBy != userId && user.Role.RoleName != "Admin" && user.Role.RoleName != "Manager")
			{
				return false;
			}

			campaign.Name = request.Name ?? string.Empty;
			campaign.VaccineName = request.VaccineName ?? string.Empty;
			campaign.EXP = request.EXP;
			campaign.MFG = request.MFG;
			campaign.VaccineType = request.VaccineType ?? string.Empty;
			campaign.StartDate = request.StartDate;
			campaign.LastUpdatedBy = userId;
			campaign.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.VaccinationCampaignRepository.Update(campaign);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteVaccinationCampaignAsync(string vaccinationCampaignId, string userId)
		{
			var campaign = _repositoryManager.VaccinationCampaignRepository
				.FindByCondition(vc => vc.Id == vaccinationCampaignId && vc.Status == ApprovalStatus.Pending, true)
				.FirstOrDefault();
			if (campaign == null) return false;

			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false).FirstOrDefault();
			if (campaign.CreatedBy != userId && user?.Role.RoleName != "Admin" && user?.Role.RoleName != "Manager")
			{
				return false;
			}

			campaign.DeletedBy = userId;
			campaign.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.VaccinationCampaignRepository.Update(campaign);
			await _repositoryManager.SaveAsync();
			return true;
		}
	}
}
