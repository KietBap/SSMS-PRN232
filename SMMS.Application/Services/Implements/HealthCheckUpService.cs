using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Enum;
using SMMS.Domain.Interface.Repositories;
namespace SMMS.Application.Services.Implements
{
	public class HealthCheckupService : IHealthCheckupService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly INotificationService _notificationService;

		public HealthCheckupService(IRepositoryManager repositoryManager, INotificationService notificationService)
		{
			_repositoryManager = repositoryManager;
			_notificationService = notificationService;
		}
		public async Task<List<HealthCheckUpResponse>> GetCheckingByParent(string parentId)
		{
			var healthCheckups = await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.Student.ParentId == parentId && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.Join(_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = hcr.Id,
						HealthActivityId = hcr.HealthActivityId, 
						StudentId = hcr.StudentId,
						StudentName = hcr.Student.FullName,
						NurseId = hcr.LastUpdatedBy,
						NurseName = user.FullName, 
						Vision = hcr.Vision,
						Hearing = hcr.Hearing,
						Dental = hcr.Dental,
						BMI = hcr.BMI,
						AbnormalNote = hcr.AbnormalNote,
						RecordDate = hcr.RecordDate,
						CheckingStatus = hcr.CheckingStatus
					})
				.ToListAsync();

			return healthCheckups;
		}

		public async Task<List<HealthCheckUpResponse>> GetCheckingByNurse(string nurseId)
		{
			var healthCheckups = await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.LastUpdatedBy == nurseId && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.Join(_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = hcr.Id,
						HealthActivityId = hcr.HealthActivityId,
						StudentId = hcr.StudentId,
						StudentName = hcr.Student.FullName,
						NurseId = hcr.LastUpdatedBy,
						NurseName = user.FullName, 
						Vision = hcr.Vision,
						Hearing = hcr.Hearing,
						Dental = hcr.Dental,
						BMI = hcr.BMI,
						AbnormalNote = hcr.AbnormalNote,
						RecordDate = hcr.RecordDate,
						CheckingStatus = hcr.CheckingStatus
					})
				.ToListAsync();

			return healthCheckups;
		}
		public async Task<bool> UpdateCheckupRecordAsync(string healthCheckupRecordId, HealthCheckupUpdateRequest request, string nurseId)
		{
			var record = _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.Id == healthCheckupRecordId, true)
				.FirstOrDefault();
			if (record == null) return false;

			record.Vision = request.Vision ?? string.Empty;
			record.Hearing = request.Hearing ?? string.Empty;
			record.Dental = request.Dental ?? string.Empty;
			record.BMI = request.BMI;
			record.AbnormalNote = request.AbnormalNote ?? string.Empty;
			record.RecordDate = DateTimeOffset.UtcNow.DateTime;
			record.CheckingStatus = request.CheckingStatus;
			record.LastUpdatedBy = nurseId;
			record.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.HealthCheckRepository.Update(record);

			await UpdateHealthProfileAsync(record);

			// Notify Parent///////////////////////////////////////////////////////
			var student = await _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == record.StudentId && s.DeletedTime == null, false)
				.FirstOrDefaultAsync();
			if (student != null)
			{
				await _notificationService.CreateNotificationAsync(
					student.ParentId,
					"New Health Checkup Record",
					$"A new health checkup record for {student.FullName} is available."
					, record.Id
				);
			}

			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task UpdateHealthProfileAsync(HealthCheckupRecord record)
		{
			var oldProfiles = await Task.Run(() => _repositoryManager.HealthProfileRepository
				.FindByCondition(hp => hp.StudentId == record.StudentId && hp.DeletedTime == null, true)
				.ToList());

			foreach (var oldProfile in oldProfiles)
			{
				oldProfile.DeletedBy = "System";
				oldProfile.DeletedTime = DateTimeOffset.UtcNow;
				_repositoryManager.HealthProfileRepository.Update(oldProfile);
			}

			var newProfile = new HealthProfile
			{
				StudentId = record.StudentId,
				Vision = record.Vision,
				Hearing = record.Hearing,
				Dental = record.Dental,
				BMI = record.BMI,
				AbnormalNote = record.AbnormalNote,
				CreatedBy = "System",
				CreatedTime = DateTimeOffset.UtcNow
			};
			_repositoryManager.HealthProfileRepository.Create(newProfile);
		}
		public async Task<List<HealthCheckUpResponse>> GetCheckupRecordsBySIdAsync(string studentId)
		{
			return await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.StudentId == studentId && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.Join(_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = hcr.Id,
						HealthActivityId = hcr.HealthActivityId,
						StudentId = hcr.StudentId,
						StudentName = hcr.Student.FullName,
						NurseId = hcr.LastUpdatedBy,
						NurseName = user.FullName,
						Vision = hcr.Vision,
						Hearing = hcr.Hearing,
						Dental = hcr.Dental,
						BMI = hcr.BMI,
						AbnormalNote = hcr.AbnormalNote,
						RecordDate = hcr.RecordDate,
						Time = hcr.Time,
						IsLatest = hcr.IsLatest,
						CheckingStatus = hcr.CheckingStatus
					})
				.ToListAsync();
		}

		public async Task<List<HealthCheckUpResponse>> GetAllCheckupRecordsAsync()
		{
			return await _repositoryManager.HealthCheckRepository
				.FindAll(false)
				.Include(hcr => hcr.Student)
				.GroupJoin(
					_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, users) => new { hcr, users }
				)
				.SelectMany(
					x => x.users.DefaultIfEmpty(),
					(x, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = x.hcr.Id,
						HealthActivityId = x.hcr.HealthActivityId,
						StudentId = x.hcr.StudentId,
						StudentName = x.hcr.Student.FullName,
						NurseId = x.hcr.LastUpdatedBy,
						NurseName = user != null ? user.FullName : "Pending To Update",
						Vision = x.hcr.Vision,
						Hearing = x.hcr.Hearing,
						Dental = x.hcr.Dental,
						BMI = x.hcr.BMI,
						AbnormalNote = x.hcr.AbnormalNote,
						RecordDate = x.hcr.RecordDate,
						Time = x.hcr.Time,
						IsLatest = x.hcr.IsLatest,
						CheckingStatus = x.hcr.CheckingStatus
					}
				)
				.ToListAsync();
		}
		public async Task<List<HealthCheckUpResponse>> GetCheckupRecordsByIdAndDateAsync(string activityId, DateTime date)
		{
			return await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.HealthActivityId == activityId && hcr.Time.Date == date.Date && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.GroupJoin(
					_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, users) => new { hcr, users }
				)
				.SelectMany(
					x => x.users.DefaultIfEmpty(),
					(x, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = x.hcr.Id,
						HealthActivityId = x.hcr.HealthActivityId,
						StudentId = x.hcr.StudentId,
						StudentName = x.hcr.Student.FullName,
						NurseId = x.hcr.LastUpdatedBy,
						NurseName = user != null ? user.FullName : "Pending To Update",
						Vision = x.hcr.Vision,
						Hearing = x.hcr.Hearing,
						Dental = x.hcr.Dental,
						BMI = x.hcr.BMI,
						AbnormalNote = x.hcr.AbnormalNote,
						RecordDate = x.hcr.RecordDate,
						Time = x.hcr.Time,
						IsLatest = x.hcr.IsLatest,
						CheckingStatus = x.hcr.CheckingStatus
					}
				)
				.ToListAsync();
		}

		public async Task<List<HealthCheckUpResponse>> GetAbnormalCheckupRecordsAsync()
		{
			return await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.CheckingStatus == CheckingStatus.Abnormal && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.GroupJoin(
					_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, users) => new { hcr, users }
				)
				.SelectMany(
					x => x.users.DefaultIfEmpty(),
					(x, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = x.hcr.Id,
						HealthActivityId = x.hcr.HealthActivityId,
						StudentId = x.hcr.StudentId,
						StudentName = x.hcr.Student.FullName,
						NurseId = x.hcr.LastUpdatedBy,
						NurseName = user != null ? user.FullName : "Pending To Update",
						Vision = x.hcr.Vision,
						Hearing = x.hcr.Hearing,
						Dental = x.hcr.Dental,
						BMI = x.hcr.BMI,
						AbnormalNote = x.hcr.AbnormalNote,
						RecordDate = x.hcr.RecordDate,
						Time = x.hcr.Time,
						IsLatest = x.hcr.IsLatest,
						CheckingStatus = x.hcr.CheckingStatus
					}
				)
				.ToListAsync();
		}
		public async Task<List<HealthCheckUpResponse>> GetNormalCheckupRecordsAsync()
		{
			return await _repositoryManager.HealthCheckRepository
				.FindByCondition(hcr => hcr.CheckingStatus == CheckingStatus.Normal && hcr.DeletedTime == null, false)
				.Include(hcr => hcr.Student)
				.GroupJoin(
					_repositoryManager.UserRepository.FindByCondition(u => u.DeletedTime == null, false),
					hcr => hcr.LastUpdatedBy,
					user => user.Id,
					(hcr, users) => new { hcr, users }
				)
				.SelectMany(
					x => x.users.DefaultIfEmpty(),
					(x, user) => new HealthCheckUpResponse
					{
						HealthCheckUpId = x.hcr.Id,
						HealthActivityId = x.hcr.HealthActivityId,
						StudentId = x.hcr.StudentId,
						StudentName = x.hcr.Student.FullName,
						NurseId = x.hcr.LastUpdatedBy,
						NurseName = user != null ? user.FullName : "Pending To Update",
						Vision = x.hcr.Vision,
						Hearing = x.hcr.Hearing,
						Dental = x.hcr.Dental,
						BMI = x.hcr.BMI,
						AbnormalNote = x.hcr.AbnormalNote,
						RecordDate = x.hcr.RecordDate,
						Time = x.hcr.Time,
						IsLatest = x.hcr.IsLatest,
						CheckingStatus = x.hcr.CheckingStatus
					}
				)
				.ToListAsync();
		}
	}
}
