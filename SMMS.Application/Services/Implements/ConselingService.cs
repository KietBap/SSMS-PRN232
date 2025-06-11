
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class ConselingService : IConselingService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ConselingService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<bool> RequestConselingScheduleAsync(string studentId, string healthCheckupId, DateTime requestedDate, string parentId, string note)
		{
			var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId && s.ParentId == parentId, false)
				.FirstOrDefault();
			if (student == null) return false;

			var healthCheckup = _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.Id == healthCheckupId, false)
				.FirstOrDefault();
			if (healthCheckup == null) return false;

			var healthActivity = _repositoryManager.HealthActivityRepository
				.FindByCondition(ha => ha.Id == healthCheckup.HealthActivityId, false)
				.FirstOrDefault();
			if (healthActivity == null) return false;

			var nurseId = healthActivity.UserId;

			var schedule = new ConselingSchedule
			{
				StudentId = studentId,
				ParentId = parentId,
				MedicalStaffId = nurseId,
				HealthCheckupId = healthCheckupId,
				MeetingDate = requestedDate,
				Note = note,
				Status = false,
				CreatedBy = parentId,
				CreatedTime = DateTimeOffset.UtcNow
			};
			_repositoryManager.ConselingRepository.Create(schedule);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> AcceptConselingScheduleAsync(string conselingScheduleId, DateTime scheduledTime, string nurseId)
		{
			var schedule = _repositoryManager.ConselingRepository
				.FindByCondition(cs => cs.Id == conselingScheduleId && cs.MedicalStaffId == nurseId, true)
				.FirstOrDefault();
			if (schedule == null) return false;

			schedule.MeetingDate = scheduledTime;
			schedule.Status = true;
			schedule.LastUpdatedBy = nurseId;
			schedule.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.ConselingRepository.Update(schedule);
			await _repositoryManager.SaveAsync();
			return true;
		}
		public async Task<List<ConselingResponse>> GetSchedulesByNIdAsync(string nurseId)
		{
			var schedules = _repositoryManager.ConselingRepository
				.FindByCondition(cs => cs.MedicalStaffId == nurseId, false)
				.ToList();
			var responses = new List<ConselingResponse>();
			foreach (var schedule in schedules)
			{
				var student = _repositoryManager.StudentRepository
					.FindByCondition(s => s.Id == schedule.StudentId, false)
					.FirstOrDefault();
				var parent = _repositoryManager.UserRepository
					.FindByCondition(u => u.Id == schedule.ParentId, false)
					.FirstOrDefault();
				var healthCheckup = _repositoryManager.HealthCheckRepository
					.FindByCondition(hcr => hcr.Id == schedule.HealthCheckupId, false)
					.FirstOrDefault();
				responses.Add(new ConselingResponse
				{
					StudentId = student?.Id,
					StudentName = student?.FullName,
					ParentName = parent?.FullName,
					HealthCheckupId = healthCheckup?.Id,
					MeetingDate = schedule.MeetingDate,
					Note = schedule.Note,
					Status = schedule.Status,
					CreatedTime = schedule.CreatedTime,
					CreatedBy = schedule.CreatedBy,
					UpdatedTime = schedule.LastUpdatedTime,
					UpdatedBy = schedule.LastUpdatedBy
				});
			}
			return responses;

		}
		public async Task<List<ConselingResponse>> GetSchedulesByPIdAsync(string parentId)
		{
			var schedules = _repositoryManager.ConselingRepository
				.FindByCondition(cs => cs.ParentId == parentId, false)
				.ToList();
			var responses = new List<ConselingResponse>();
			foreach (var schedule in schedules)
			{
				var student = _repositoryManager.StudentRepository
					.FindByCondition(s => s.Id == schedule.StudentId, false)
					.FirstOrDefault();
				var parent = _repositoryManager.UserRepository
					.FindByCondition(u => u.Id == schedule.ParentId, false)
					.FirstOrDefault();
				var healthCheckup = _repositoryManager.HealthCheckRepository
					.FindByCondition(hcr => hcr.Id == schedule.HealthCheckupId, false)
					.FirstOrDefault();
				responses.Add(new ConselingResponse
				{
					StudentId = student?.Id,
					StudentName = student?.FullName,
					ParentName = parent?.FullName,
					HealthCheckupId = healthCheckup?.Id,
					MeetingDate = schedule.MeetingDate,
					Note = schedule.Note,
					Status = schedule.Status,
					CreatedTime = schedule.CreatedTime,
					CreatedBy = schedule.CreatedBy,
					UpdatedTime = schedule.LastUpdatedTime,
					UpdatedBy = schedule.LastUpdatedBy
				});
			}
			return responses;

		}
	}
}
