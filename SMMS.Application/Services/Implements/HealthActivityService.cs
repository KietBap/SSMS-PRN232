

using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;
using SMMS.Domain.Interface.Repositories;
using SMMS.Infrastructure.Implements;

namespace SMMS.Application.Services.Implements
{
	public class HealthActivityService : IHealthActivityService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly INotificationService _notificationService;

		public HealthActivityService(IRepositoryManager repositoryManager, INotificationService notificationService)
		{
			_repositoryManager = repositoryManager;
			_notificationService = notificationService;
		}

		public async Task<HealthActivityResponse> CreateHealthActivityAsync(HealthActivityRequest request, string nurseId)
		{
			var existingClassIds = _repositoryManager.ClassRepository
			.FindByCondition(c => request.ClassIds.Contains(c.Id) && c.DeletedTime == null, false)
			.Select(c => c.Id)
			.ToList();
			if (existingClassIds.Count != request.ClassIds.Count)
			{
				throw new Exception("Một hoặc nhiều lớp không tồn tại.");
			}
			var user = await _repositoryManager.UserRepository
				.FindByCondition(u => u.Id == nurseId, false).FirstOrDefaultAsync();
			var healthActivity = new HealthActivity
			{
				UserId = nurseId,
				Name = request.Name ?? string.Empty,
				Description = request.Description ?? string.Empty,
				ScheduledDate = request.ScheduledDate,
				Status = ApprovalStatus.Pending,
				CreatedBy = nurseId,
				CreatedTime = DateTimeOffset.UtcNow,
				HealthActivityClasses = request.ClassIds.Select(classId => new HealthActivityClass
				{
					SchoolClassId = classId,
					CreatedBy = nurseId,
					CreatedTime = DateTimeOffset.UtcNow
				}).ToList()
			};
			_repositoryManager.HealthActivityRepository.Create(healthActivity);
			await _repositoryManager.SaveAsync();

			//Notification for Admin/////////////////////////////////////////////////////////
			var admins = await _repositoryManager.UserRepository
			.FindByCondition(u => u.Role.RoleName == "Admin" && u.DeletedTime == null, false)
			.ToListAsync();
			foreach (var admin in admins)
			{
				await _notificationService.CreateNotificationAsync(
					admin.Id,
					"New Health Activity Needs Approval",
					$"Activity: {healthActivity.Name} created by Nurse requires your approval.",
					healthActivity.Id
				);
			}
			////////////////////////////////////////////////////////////////////////////////
			
			return new HealthActivityResponse
			{
				Id = healthActivity.Id,
				Name = healthActivity.Name ?? string.Empty,
				UserId = healthActivity.UserId,
				UserName = user?.FullName ?? "Unknown Nurse",
				Description = healthActivity.Description ?? string.Empty,
				ScheduledDate = healthActivity.ScheduledDate,
				Status = healthActivity.Status,
				ClassIds = healthActivity.HealthActivityClasses.Select(hac => hac.SchoolClassId).ToList()
			};
		}

		private async Task CreateActivityConsentsAsync(HealthActivity healthActivity)
		{
			var classIds = healthActivity.HealthActivityClasses.Select(hac => hac.SchoolClassId).ToList();
			var students = await _repositoryManager.StudentRepository
			   .FindByCondition(s => classIds.Contains(s.ClassId) && s.DeletedTime == null, false)
			   .ToListAsync();

			foreach (var student in students)
			{
				var consent = new ActivityConsent
				{
					StudentId = student.Id,
					UserId = student.ParentId,
					HealthActivityId = healthActivity.Id,
					VaccinationCampaignId = null,
					Status = ApprovalStatus.Pending,
					Comments = healthActivity.Description,
					ScheduleTime = healthActivity.ScheduledDate,
					CreatedBy = "System",
					CreatedTime = DateTimeOffset.UtcNow,
					ActivityType = "HealthActivity"
				};
				_repositoryManager.ConsentRepository.Create(consent);
				await _repositoryManager.SaveAsync();

				// Create notification for each consent/////////////////////////////////////
				await _notificationService.CreateNotificationAsync(
					student.ParentId,
							"New Health Activity for Your Child",
							$"Activity: {healthActivity.Name} for your child: {student.FullName}. Please confirm participation.",
					consent.Id

				);
			}
		}

		public async Task<List<HealthActivityResponse>> GetPendingHealthActivitiesAsync()
		{
			return await Task.Run(() => _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Status == ApprovalStatus.Pending, false)
				.Include(ha => ha.HealthActivityClasses)
				.Include(ha => ha.User) // Include User để lấy FullName  
				.Select(ha => new HealthActivityResponse
				{
					Id = ha.Id,
					Name = ha.Name,
					UserId = ha.UserId,
					UserName = ha.User != null ? ha.User.FullName : "Unknown Nurse",
					Description = ha.Description,
					ScheduledDate = ha.ScheduledDate,
					Status = ha.Status,
					ClassIds = ha.HealthActivityClasses.Select(hac => hac.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<List<HealthActivityResponse>> GetActivityWithoutPendingAsync()
		{
			return await Task.Run(() => _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Status == ApprovalStatus.Approved || ha.Status == ApprovalStatus.Rejected, false)
				.Include(ha => ha.HealthActivityClasses)
				.Include(ha => ha.User) // Include User để lấy FullName  
				.Select(ha => new HealthActivityResponse
				{
					Id = ha.Id,
					Name = ha.Name,
					UserId = ha.UserId,
					UserName = ha.User != null ? ha.User.FullName : "Unknown Nurse",
					Description = ha.Description,
					ScheduledDate = ha.ScheduledDate,
					Status = ha.Status,
					ClassIds = ha.HealthActivityClasses.Select(hac => hac.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<List<HealthActivityResponse>> GetAllHealthActivityAsync()
		{
			return await Task.Run(() => _repositoryManager.HealthActivityRepository
				.FindAll(false)
				.Include(ha => ha.HealthActivityClasses)
				.Include(ha => ha.User) // Include User để lấy FullName  
				.Select(ha => new HealthActivityResponse
				{
					Id = ha.Id,
					Name = ha.Name,
					UserId = ha.UserId,
					UserName = ha.User != null ? ha.User.FullName : "Unknown Nurse",
					Description = ha.Description,
					ScheduledDate = ha.ScheduledDate,
					Status = ha.Status,
					ClassIds = ha.HealthActivityClasses.Select(hac => hac.SchoolClassId).ToList()
				}).ToList());
		}

		public async Task<bool> UpdateHealthActivityStatusAsync(string healthActivityId, string action, string userId)
		{
			var healthActivity = _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Id == healthActivityId && ha.Status == ApprovalStatus.Pending && ha.DeletedTime == null, true)
				.Include(ha => ha.HealthActivityClasses)
				.FirstOrDefault();
			if (healthActivity == null) return false;

			if (action == "approve")
			{
				healthActivity.Status = ApprovalStatus.Approved;
				await CreateActivityConsentsAsync(healthActivity);

				// Notify Nurse////////////////////////
				await _notificationService.CreateNotificationAsync(
					healthActivity.UserId,
					"Health Activity Approved",
					$"Your activity: {healthActivity.Name} has been approved.",
					healthActivity.Id
				);
			}
			else if (action == "reject")
			{
				healthActivity.Status = ApprovalStatus.Rejected;
			}
			else
			{
				return false;
			}

			healthActivity.LastUpdatedBy = userId;
			healthActivity.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.HealthActivityRepository.Update(healthActivity);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateHealthActivityAsync(string healthActivityId, HealthActivityRequest request, string userId)
		{
			var activity = _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Id == healthActivityId 
					&& ha.Status == ApprovalStatus.Pending, true)
				.FirstOrDefault();
			if (activity == null) return false;

			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false).FirstOrDefault();
			if (user == null) return false;
			if (activity.UserId != userId && user.Role.RoleName != "Admin" && user.Role.RoleName != "Manager")
			{
				return false;
			}

			activity.Name = request.Name ?? string.Empty;
			activity.Description = request.Description ?? string.Empty;
			activity.ScheduledDate = request.ScheduledDate;
			activity.LastUpdatedBy = userId;
			activity.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.HealthActivityRepository.Update(activity);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteHealthActivityAsync(string healthActivityId, string userId)
		{
			var activity = _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Id == healthActivityId && ha.Status == ApprovalStatus.Pending || ha.Status == ApprovalStatus.Rejected, true)
				.FirstOrDefault();
			if (activity == null) return false;
			
			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false).FirstOrDefault();
			if (user == null) return false;
			if (activity.UserId != userId && user.Role.RoleName != "Admin" && user.Role.RoleName != "Manager")
			{
				return false;
			}
			activity.DeletedBy = userId;
			activity.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.HealthActivityRepository.Update(activity);
			await _repositoryManager.SaveAsync();
			return true;
		}	
	}
}
