using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class VaccinationRecordService : IVaccinationRecordService
	{
		private readonly IRepositoryManager _repositoryManager;

		public VaccinationRecordService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<bool> UpdateVaccinationRecordAsync(string vaccinationRecordId, VaccinationRecordRequest request, string nurseId)
		{
			var record = _repositoryManager.VaccinationRecordRepository
				.FindByCondition(vr => vr.Id == vaccinationRecordId, true)
				.FirstOrDefault();
			if (record == null) return false;

			record.ResultNote = request.ResultNote;
			record.VaccinatedAt = DateTimeOffset.UtcNow.DateTime;
			record.LastUpdatedBy = nurseId;
			record.LastUpdatedTime = DateTimeOffset.UtcNow;
			_repositoryManager.VaccinationRecordRepository.Update(record);
			await _repositoryManager.SaveAsync();
			return true;
		}
		public async Task<List<VaccinationRecordResponse>> GetVaccinationRecordsByStudentIdAsync(string studentId)
		{
			var records = await Task.Run(() => _repositoryManager.VaccinationRecordRepository
				.FindByCondition(vr => vr.StudentId == studentId && vr.DeletedTime == null, false)
				.Include(vr => vr.Student)
				.Include(vr => vr.VaccinationCampaign)
				.Select(vr => new VaccinationRecordResponse
				{
					Id = vr.Id,
					StudentId = vr.StudentId,
					StudentName = vr.Student.FullName,
					VaccinationCampaignId = vr.VaccinationCampaignId,
					VaccineName = vr.VaccinationCampaign.VaccineName,
					ResultNote = vr.ResultNote,
					Time = vr.Time,
					VaccinatedAt = vr.VaccinatedAt
				}).ToList());

			return records;
		}

		public async Task<List<VaccinationRecordResponse>> GetAllVaccinationRecordsAsync()
		{
			return await Task.Run(() => _repositoryManager.VaccinationRecordRepository
				.FindAll(false)
				.Include(vr => vr.Student)
				.Include(vr => vr.VaccinationCampaign)
				.Select(vr => new VaccinationRecordResponse
				{
					Id = vr.Id,
					StudentId = vr.StudentId,
					StudentName = vr.Student.FullName,
					VaccinationCampaignId = vr.VaccinationCampaignId,
					VaccineName = vr.VaccinationCampaign.VaccineName,
					ResultNote = vr.ResultNote,
					Time = vr.Time,
					VaccinatedAt = vr.VaccinatedAt
				}).ToList());
		}
		public async Task<List<VaccinationRecordResponse>> GetVaccineRecordsByDateAsync(DateTime date)
		{
			return await Task.Run(() => _repositoryManager.VaccinationRecordRepository
				.FindByCondition(vr => vr.Time.Date == date.Date && vr.DeletedTime == null, false)
				.Include(vr => vr.Student)
				.Include(vr => vr.VaccinationCampaign)
				.Select(vr => new VaccinationRecordResponse
				{
					Id = vr.Id,
					StudentId = vr.StudentId,
					StudentName = vr.Student.FullName,
					VaccinationCampaignId = vr.VaccinationCampaignId,
					VaccineName = vr.VaccinationCampaign.VaccineName,
					ResultNote = vr.ResultNote,
					Time = vr.Time,
					VaccinatedAt = vr.VaccinatedAt
				}).ToList());
		}
	}
}
