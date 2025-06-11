

using ClosedXML.Excel;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class NurseService : INurseService
	{
		private readonly IRepositoryManager _repositoryManager;

		public NurseService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<bool> CreateHealthProfileAsync(string studentId, HealthProfileRequest request)
		{
			// Kiểm tra học sinh có tồn tại không
			var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId && s.DeletedTime == null, false)
				.FirstOrDefault();
			if (student == null) return false;

			var healthProfile = new HealthProfile
			{
				StudentId = studentId,
				Vision = request.Vision ?? string.Empty,
				Hearing = request.Hearing ?? string.Empty,
				Dental = request.Dental ?? string.Empty,
				BMI = request.BMI,
				AbnormalNote = request.AbnormalNote,
				VaccinationHistory = request.VaccinationHistory,
				CreatedBy = "Nurse",
				CreatedTime = DateTimeOffset.UtcNow
			};

			_repositoryManager.HealthProfileRepository.Create(healthProfile);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<HealthProfileResponse> GetHealthProfileByStudentIdAsync(string studentId)
		{
			var healthProfile = await Task.Run(() => _repositoryManager.HealthProfileRepository
				.FindByCondition(hp => hp.StudentId == studentId && hp.DeletedTime == null, false)
				.Select(hp => new HealthProfileResponse
				{
					Id = hp.Id,
					StudentId = hp.StudentId,
					Vision = hp.Vision,
					Hearing = hp.Hearing,
					Dental = hp.Dental,
					BMI = hp.BMI,
					AbnormalNote = hp.AbnormalNote,
					VaccinationHistory = hp.VaccinationHistory
				}).FirstOrDefault());
			return healthProfile;
		}

		public async Task<bool> UpdateHealthProfileAsync(string studentId, HealthProfileRequest request)
		{
			var healthProfile = _repositoryManager.HealthProfileRepository
				.FindByCondition(hp => hp.StudentId == studentId && hp.DeletedTime == null, true)
				.FirstOrDefault();
			if (healthProfile == null) return false;

			healthProfile.Vision = request.Vision ?? string.Empty;
			healthProfile.Hearing = request.Hearing ?? string.Empty;
			healthProfile.Dental = request.Dental ?? string.Empty;
			healthProfile.BMI = request.BMI;
			healthProfile.AbnormalNote = request.AbnormalNote;
			healthProfile.VaccinationHistory = request.VaccinationHistory;
			healthProfile.LastUpdatedBy = "Nurse";
			healthProfile.LastUpdatedTime = DateTimeOffset.UtcNow;

			_repositoryManager.HealthProfileRepository.Update(healthProfile);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteHealthProfileAsync(string studentId)
		{
			var healthProfile = _repositoryManager.HealthProfileRepository
				.FindByCondition(hp => hp.StudentId == studentId && hp.DeletedTime == null, true)
				.FirstOrDefault();
			if (healthProfile == null) return false;

			healthProfile.DeletedBy = "Nurse";
			healthProfile.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.HealthProfileRepository.Update(healthProfile);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task ImportHealthProfilesFromExcelAsync(Stream fileStream)
		{
			using var workbook = new XLWorkbook(fileStream);
			var worksheet = workbook.Worksheet(1); // Giả sử dữ liệu ở sheet đầu tiên
			var rows = worksheet.RowsUsed().Skip(1); // Bỏ qua hàng tiêu đề

			foreach (var row in rows)
			{
				var studentId = row.Cell(1).GetString();
				var vision = row.Cell(2).GetString();
				var hearing = row.Cell(3).GetString();
				var dental = row.Cell(4).GetString();
				var bmi = row.Cell(5).GetDouble();
				var abnormalNote = row.Cell(6).GetString();
				var vaccinationHistory = row.Cell(7).GetString();

				// Kiểm tra học sinh có tồn tại không
				var student = _repositoryManager.StudentRepository
					.FindByCondition(s => s.Id == studentId && s.DeletedTime == null, false)
					.FirstOrDefault();
				if (student == null) continue;

				var healthProfile = _repositoryManager.HealthProfileRepository
					.FindByCondition(hp => hp.StudentId == studentId && hp.DeletedTime == null, true)
					.FirstOrDefault();

				if (healthProfile == null)
				{
					// Tạo mới nếu chưa có
					healthProfile = new HealthProfile
					{
						StudentId = studentId,
						Vision = vision,
						Hearing = hearing,
						Dental = dental,
						BMI = bmi,
						AbnormalNote = abnormalNote,
						VaccinationHistory = vaccinationHistory,
						CreatedBy = "Nurse",
						CreatedTime = DateTimeOffset.UtcNow
					};
					_repositoryManager.HealthProfileRepository.Create(healthProfile);
				}
				else
				{
					// Cập nhật nếu đã có
					healthProfile.Vision = vision;
					healthProfile.Hearing = hearing;
					healthProfile.Dental = dental;
					healthProfile.BMI = bmi;
					healthProfile.AbnormalNote = abnormalNote;
					healthProfile.VaccinationHistory = vaccinationHistory;
					healthProfile.LastUpdatedBy = "Nurse";
					healthProfile.LastUpdatedTime = DateTimeOffset.UtcNow;
					_repositoryManager.HealthProfileRepository.Update(healthProfile);
				}
			}
			await _repositoryManager.SaveAsync();
		}
	}
}
