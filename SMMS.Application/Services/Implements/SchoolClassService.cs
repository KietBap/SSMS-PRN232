using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class SchoolClassService : ISchoolClassService
	{
		private readonly IRepositoryManager _repositoryManager;

		public SchoolClassService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<List<SchoolClassResponse>> GetAllSchoolClassesAsync()
		{
			var classes = await _repositoryManager.ClassRepository
				.FindByCondition(c => c.DeletedTime == null, false)
				// EF Core ≥ 5 cho phép Include có điều kiện
				.Include(c => c.Students.Where(s => s.DeletedTime == null))
				.Select(c => new SchoolClassResponse
				{
					Id = c.Id,
					ClassName = c.ClassName,
					ClassRoom = c.ClassRoom,
					Quantity = c.Quantity,
					Students = c.Students
								.Where(s => s.DeletedTime == null)
								.Select(s => new StudentResponse
								{
									Id = s.Id,
									StudentCode = s.StudentCode,
									FullName = s.FullName,
									Gender = s.Gender,
									DateOfBirth = s.DateOfBirth,
									Image = s.Image
								}).ToList()
				})
				.ToListAsync();   // nhớ await
			return classes;
		}


		public async Task<SchoolClassResponse> GetSchoolClassByIdAsync(string id)
		{
			var schoolClass = _repositoryManager.ClassRepository.FindByCondition(c => c.Id == id && c.DeletedTime == null, true)
				.Include(c => c.Students.Where(s => s.DeletedTime == null))
				.Select(c => new SchoolClassResponse
				{
					Id = c.Id,
					ClassName = c.ClassName,
					ClassRoom = c.ClassRoom,
					Quantity = c.Quantity,
					Students = c.Students
								.Where(s => s.DeletedTime == null)
								.Select(s => new StudentResponse
								{
									Id = s.Id,
									StudentCode = s.StudentCode,
									FullName = s.FullName,
									Gender = s.Gender,
									DateOfBirth = s.DateOfBirth,
									Image = s.Image
								}).ToList()
				}).FirstOrDefault();
			return schoolClass;
		}

		public async Task<bool> CreateSchoolClassAsync(SchoolClassRequest request)
		{
			var schoolClass = new SchoolClass
			{
				ClassName = request.ClassName,
				ClassRoom = request.ClassRoom,
				Quantity = request.Quantity,
				CreatedBy = "Admin",
				CreatedTime = DateTimeOffset.UtcNow
			};
			_repositoryManager.ClassRepository.Create(schoolClass);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateSchoolClassAsync(string id, SchoolClassRequest request)
		{
			var schoolClass = _repositoryManager.ClassRepository.FindByCondition(c => c.Id == id && c.DeletedTime == null, true)
				.FirstOrDefault();
			if (schoolClass == null) return false;

			schoolClass.ClassName = request.ClassName;
			schoolClass.ClassRoom = request.ClassRoom;
			schoolClass.Quantity = request.Quantity;
			schoolClass.LastUpdatedBy = "Admin";
			schoolClass.LastUpdatedTime = DateTimeOffset.UtcNow;

			_repositoryManager.ClassRepository.Update(schoolClass);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteSchoolClassAsync(string id)
		{
			var schoolClass = _repositoryManager.ClassRepository.FindByCondition(c => c.Id == id && c.DeletedTime == null, true)
				.FirstOrDefault();
			if (schoolClass == null) return false;

			schoolClass.DeletedBy = "Admin";
			schoolClass.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.ClassRepository.Update(schoolClass);
			await _repositoryManager.SaveAsync();
			return true;
		}
	}
}
