using Microsoft.EntityFrameworkCore;
using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Helpers.Implements;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;
using System.Linq.Expressions;

namespace SMMS.Application.Services.Implements
{
	public class UserService : IUserService
	{
		private readonly IRepositoryManager _repositoryManager;
		private readonly CloudinaryService _cloudinaryService;

		public UserService(IRepositoryManager repositoryManager, CloudinaryService cloudinaryService)
		{
			_repositoryManager = repositoryManager;
			_cloudinaryService = cloudinaryService;
		}

		public async Task<List<UserResponse>> GetAllUsersAsync()
		{
			var users = await Task.Run(() => _repositoryManager.UserRepository.FindAll(false)
				.Select(u => new UserResponse
				{
					Id = u.Id,
					Email = u.Email,
					Phone = u.Phone,
					FullName = u.FullName,
					RoleName = u.Role.RoleName
				}).ToList());
			return users;
		}

		public async Task<UserResponse> GetUserByIdAsync(string id)
		{
			var user = await Task.Run(() => _repositoryManager.UserRepository.FindByCondition(u => u.Id == id, false)
				.Select(u => new UserResponse
				{
					Id = u.Id,
					Email = u.Email,
					Phone = u.Phone,
					FullName = u.FullName,
					RoleName = u.Role.RoleName
				}).FirstOrDefault());
			return user;
		}

		public async Task<bool> CreateUserAsync(UserCreateRequest request)
		{
			var user = new User
			{
				Email = request.Email ?? string.Empty,
				Phone = request.Phone ?? string.Empty,
				FullName = request.FullName ?? string.Empty,
				RoleId = request.RoleId ?? string.Empty,
				Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
				CreatedBy = "Admin",
				CreatedTime = DateTimeOffset.UtcNow
			};
			_repositoryManager.UserRepository.Create(user);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateUserAsync(string id, UserUpdateRequest request)
		{
			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == id, true)
				.FirstOrDefault();
			if (user == null) return false;

			user.Phone = request.Phone ?? string.Empty;
			user.FullName = request.FullName ?? string.Empty;
			if (!string.IsNullOrEmpty(request.Password))
				user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
			user.LastUpdatedBy = "Admin";
			user.LastUpdatedTime = DateTimeOffset.UtcNow;

			_repositoryManager.UserRepository.Update(user);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteUserAsync(string id)
		{
			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == id, true)
				.FirstOrDefault();
			if (user == null) return false;

			user.DeletedBy = "Admin";
			user.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.UserRepository.Update(user);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<UserProfileResponse> GetMyProfileAsync(string userId)
		{
			var user = await Task.Run(() => _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, false)
				.Select(u => new UserProfileResponse
				{
					Id = u.Id,
					Email = u.Email,
					Phone = u.Phone,
					FullName = u.FullName,
					Image = u.Image
				}).FirstOrDefault());
			return user ?? throw new Exception("User not found");
		}

		public async Task<bool> UpdateMyProfileAsync(string userId, UserProfileUpdateRequest request)
		{
			var user = _repositoryManager.UserRepository.FindByCondition(u => u.Id == userId, true)
				.FirstOrDefault();
			if (user == null) return false;

			user.FullName = request.FullName ?? string.Empty;
			user.Phone = request.Phone ?? string.Empty;

			if (request.Image != null)
			{
				var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
				if (!string.IsNullOrEmpty(imageUrl))
				{
					user.Image = imageUrl;
				}
			}

			user.LastUpdatedBy = userId;
			user.LastUpdatedTime = DateTimeOffset.UtcNow;

			_repositoryManager.UserRepository.Update(user);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<List<StudentResponse>> GetMyStudentsAsync(string parentId)
		{
			var students = await Task.Run(() => _repositoryManager.StudentRepository
				.FindByCondition(s => s.ParentId == parentId && s.DeletedTime == null, false)
				.Include(s => s.SchoolClass)
				.Include(s => s.HealthProfiles)
				.Include(s => s.HealthCheckupRecords)
				.ToList()
				.Select(s => new StudentResponse
				{
					Id = s.Id,
					StudentCode = s.StudentCode,
					FullName = s.FullName,
					Gender = s.Gender,
					DateOfBirth = s.DateOfBirth,
					ClassId = s.ClassId,
					StudentClass = s.SchoolClass != null ? new SchoolClassResponse
					{
						Id = s.SchoolClass.Id,
						ClassName = s.SchoolClass.ClassName,
						ClassRoom = s.SchoolClass.ClassRoom,
						Quantity = s.SchoolClass.Quantity
					} : null,
					Image = s.Image,
					HealthProfile = s.HealthProfiles
						.Where(hp => hp.DeletedTime == null)
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
						}).FirstOrDefault(),
					HealthCheckupRecords = s.HealthCheckupRecords
						.Where(hcr => hcr.DeletedTime == null)
						.Select(hcr => new HealthCheckUpResponse
						{
							HealthActivityId = hcr.HealthActivityId,
							StudentId = hcr.StudentId,
							StudentName = hcr.Student.FullName,
							NurseId = hcr.LastUpdatedBy,
							NurseName = _repositoryManager.UserRepository
								.FindByCondition(u => u.Id == hcr.LastUpdatedBy, false)
								.Select(u => u.FullName)
								.FirstOrDefault(),
							Vision = hcr.Vision,
							Hearing = hcr.Hearing,
							Dental = hcr.Dental,
							BMI = hcr.BMI,
							AbnormalNote = hcr.AbnormalNote,
							Time = hcr.RecordDate,
							RecordDate = hcr.RecordDate,
							IsLatest = hcr.IsLatest,
							CheckingStatus = hcr.CheckingStatus
						}).ToList()
				}).ToList());

			return students;
		}

		public async Task<List<StudentResponse>> GetAllStudentsAsync()
		{
			var students = await Task.Run(() => _repositoryManager.StudentRepository
				.FindByCondition(s => s.DeletedTime == null, false)
				.Include(s => s.SchoolClass)
				.Include(s => s.HealthProfiles)
				.Include(s => s.HealthCheckupRecords)
				.Select(s => new StudentResponse
				{
					Id = s.Id,
					StudentCode = s.StudentCode,
					FullName = s.FullName,
					Gender = s.Gender,
					DateOfBirth = s.DateOfBirth,
					ClassId = s.ClassId,
					StudentClass = s.SchoolClass != null ? new SchoolClassResponse
					{
						Id = s.SchoolClass.Id,
						ClassName = s.SchoolClass.ClassName,
						ClassRoom = s.SchoolClass.ClassRoom,
						Quantity = s.SchoolClass.Quantity,
					} : null,
					Image = s.Image,
					HealthProfile = s.HealthProfiles
						.Where(hp => hp.DeletedTime == null)
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
						}).FirstOrDefault(),
					HealthCheckupRecords = s.HealthCheckupRecords
						.Where(hcr => hcr.DeletedTime == null)
						.Select(hcr => new HealthCheckUpResponse
						{
							HealthActivityId = hcr.HealthActivityId,
							StudentId = hcr.StudentId,
							StudentName = hcr.Student.FullName,
							NurseId = hcr.LastUpdatedBy,
							Vision = hcr.Vision,
							Hearing = hcr.Hearing,
							Dental = hcr.Dental,
							BMI = hcr.BMI,
							AbnormalNote = hcr.AbnormalNote,
							Time = hcr.RecordDate,
							RecordDate = hcr.RecordDate,
							IsLatest = hcr.IsLatest,
							CheckingStatus = hcr.CheckingStatus
						}).ToList()
				}).ToList());

			return students;
		}

		public async Task<StudentResponse> GetStudentByIdAsync(string id)
		{
			var student = await _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == id && s.DeletedTime == null, false)
				.Include(s => s.SchoolClass)
				.Include(s => s.HealthProfiles)
				.Include(s => s.HealthCheckupRecords)
				.Select(s => new StudentResponse
				{
					Id = s.Id,
					StudentCode = s.StudentCode,
					FullName = s.FullName,
					Gender = s.Gender,
					DateOfBirth = s.DateOfBirth,
					ClassId = s.ClassId,
					StudentClass = s.SchoolClass != null ? new SchoolClassResponse
					{
						Id = s.SchoolClass.Id,
						ClassName = s.SchoolClass.ClassName,
						ClassRoom = s.SchoolClass.ClassRoom,
						Quantity = s.SchoolClass.Quantity
					} : null,
					Image = s.Image,
					HealthProfile = s.HealthProfiles
						.Where(hp => hp.DeletedTime == null)
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
						}).FirstOrDefault(),
					HealthCheckupRecords = s.HealthCheckupRecords
						.Where(hcr => hcr.DeletedTime == null)
						.Select(hcr => new HealthCheckUpResponse
						{
							HealthActivityId = hcr.HealthActivityId,
							StudentId = hcr.StudentId,
							StudentName = hcr.Student.FullName,
							NurseId = hcr.LastUpdatedBy,
							Vision = hcr.Vision,
							Hearing = hcr.Hearing,
							Dental = hcr.Dental,
							BMI = hcr.BMI,
							AbnormalNote = hcr.AbnormalNote,
							Time = hcr.RecordDate,
							RecordDate = hcr.RecordDate,
							IsLatest = hcr.IsLatest,
							CheckingStatus = hcr.CheckingStatus
						}).ToList()
				}).FirstOrDefaultAsync();

			return student;
		}

		public async Task<bool> CreateStudentAsync(string parentId, StudentRequest request)
		{
			var parent = _repositoryManager.UserRepository
				.FindByCondition(u => u.Id == parentId && u.Role.RoleName == "Parent", false)
				.FirstOrDefault();
			if (parent == null) return false;

			var student = new Student
			{
				ParentId = parentId,
				ClassId = request.ClassId ?? string.Empty,
				FullName = request.FullName ?? string.Empty,
				Gender = request.Gender ?? string.Empty ,
				DateOfBirth = request.DateOfBirth,
				CreatedBy = parentId,
				CreatedTime = DateTimeOffset.UtcNow
			};

			if (request.Image != null)
			{
				var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
				if (!string.IsNullOrEmpty(imageUrl))
				{
					student.Image = imageUrl;
				}
			}

			_repositoryManager.StudentRepository.Create(student);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateStudentAsync(string studentId, string userId, StudentRequest request)
		{
			var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId, true)

				.FirstOrDefault();
			if (student == null) return false;

			var user = _repositoryManager.UserRepository
				.FindByCondition(u => u.Id == userId, false)
				.Include(u => u.Role)
				.FirstOrDefault();
			if (user == null || (user.Role.RoleName != "Admin" && student.ParentId != userId))
				return false;

			if (request.FullName != null) student.FullName = request.FullName;
			if (request.Gender != null) student.Gender = request.Gender;
			if (request.DateOfBirth != default) student.DateOfBirth = request.DateOfBirth;
			if (request.ClassId != null) student.ClassId = request.ClassId;

			if (request.Image != null)
			{
				var imageUrl = await _cloudinaryService.UploadImageAsync(request.Image);
				if (!string.IsNullOrEmpty(imageUrl))
				{
					student.Image = imageUrl;
				}
			}

			student.LastUpdatedBy = userId;
			student.LastUpdatedTime = DateTimeOffset.UtcNow;

			//_repositoryManager.StudentRepository.Update(student);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteStudentAsync(string studentId, string userId)
		{
			var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId, true)
				.FirstOrDefault();
			if (student == null) return false;

			var user = _repositoryManager.UserRepository
				.FindByCondition(u => u.Id == userId, false)
				.Include(u => u.Role)
				.FirstOrDefault();
			if (user == null || (user.Role.RoleName != "Admin" && student.ParentId != userId))
				return false;

			student.DeletedBy = userId;
			student.DeletedTime = DateTimeOffset.UtcNow;

			_repositoryManager.StudentRepository.Update(student);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateHealthProfileByParentAsync(string studentId, HealthProfileRequest request, string parentId)
		{
			// Kiểm tra xem học sinh có tồn tại và thuộc về phụ huynh này không
			var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId && s.ParentId == parentId && s.DeletedTime == null, false)
				.FirstOrDefault();
			if (student == null) return false;

			// Lấy hồ sơ sức khỏe hiện tại của học sinh (chưa bị xóa mềm)
			var healthProfile = _repositoryManager.HealthProfileRepository
				.FindByCondition(hp => hp.StudentId == studentId && hp.DeletedTime == null, true)
				.FirstOrDefault();

			if (healthProfile == null)
			{
				// Nếu chưa có hồ sơ sức khỏe, tạo mới
				healthProfile = new HealthProfile
				{
					StudentId = studentId,
					Vision = request.Vision ?? string.Empty,
					Hearing = request.Hearing ?? string.Empty,
					Dental = request.Dental ?? string.Empty,
					BMI = request.BMI,
					AbnormalNote = request.AbnormalNote,
					VaccinationHistory = request.VaccinationHistory,
					CreatedBy = parentId,
					CreatedTime = DateTimeOffset.UtcNow
				};
				_repositoryManager.HealthProfileRepository.Create(healthProfile);
			}
			else
			{
				// Nếu đã có, cập nhật thông tin
				healthProfile.Vision = request.Vision ?? string.Empty;
				healthProfile.Hearing = request.Hearing ?? string.Empty;
				healthProfile.Dental = request.Dental ?? string.Empty;
				healthProfile.BMI = request.BMI;
				healthProfile.AbnormalNote = request.AbnormalNote;
				healthProfile.VaccinationHistory = request.VaccinationHistory;
				healthProfile.LastUpdatedBy = parentId;
				healthProfile.LastUpdatedTime = DateTimeOffset.UtcNow;
				_repositoryManager.HealthProfileRepository.Update(healthProfile);
			}

			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<List<ParentResponse>> GetAllParentsAsync()
		{
			return await _repositoryManager.UserRepository
				.FindByCondition(u => u.Role.RoleName == "Parent" && u.DeletedTime == null, false)
				.Include(u => u.Role)
				.Include(u => u.Students.Where(s => s.DeletedTime == null))
				.Select(u => new ParentResponse
				{
					Id = u.Id,
					Email = u.Email,
					Phone = u.Phone,
					FullName = u.FullName,
					RoleName = u.Role.RoleName,
					ImageUrl = u.Image,
					Students = u.Students.Select(s => new StudentResponse
					{
						Id = s.Id,
						FullName = s.FullName,
						Gender = s.Gender,
						DateOfBirth = s.DateOfBirth,
						ClassId = s.ClassId,
						StudentClass = s.SchoolClass != null ? new SchoolClassResponse
						{
							Id = s.SchoolClass.Id,
							ClassName = s.SchoolClass.ClassName,
							ClassRoom = s.SchoolClass.ClassRoom,
							Quantity = s.SchoolClass.Quantity
						} : null,
						Image = s.Image
					}).ToList()
				})
				.ToListAsync();
		}

		public async Task<ParentResponse> GetParentByStudentIdAsync(string studentId)
		{
			var parent = await _repositoryManager.StudentRepository
				.FindByCondition(s => s.Id == studentId && s.DeletedTime == null, false)
				.Include(s => s.Parent)
				.ThenInclude(p => p.Students.Where(st => st.DeletedTime == null))
				.Select(s => new ParentResponse
				{
					Id = s.Parent.Id,
					Email = s.Parent.Email,
					Phone = s.Parent.Phone,
					FullName = s.Parent.FullName,
					RoleName = s.Parent.Role.RoleName,
					ImageUrl = s.Parent.Image,
					Students = s.Parent.Students.Select(st => new StudentResponse
					{
						Id = st.Id,
						FullName = st.FullName,
						Gender = st.Gender,
						DateOfBirth = st.DateOfBirth,
						ClassId = st.ClassId,
						StudentClass = st.SchoolClass != null ? new SchoolClassResponse
						{
							Id = st.SchoolClass.Id,
							ClassName = st.SchoolClass.ClassName,
							ClassRoom = st.SchoolClass.ClassRoom,
							Quantity = st.SchoolClass.Quantity
						} : null,
						Image = st.Image
					}).ToList()
				}).FirstOrDefaultAsync();

			return parent ?? throw new Exception("Parent not found for the given student ID.");
		}

		public async Task<StudentResponse> GetStudentByStudentCodeAsync(string studentCode)
		{
			var student = await _repositoryManager.StudentRepository
				.FindByCondition(s => s.StudentCode == studentCode && s.DeletedTime == null, false)
				.Include(s => s.SchoolClass)
				.Include(s => s.HealthProfiles)
				.Include(s => s.HealthCheckupRecords)
				.Select(s => new StudentResponse
				{
					Id = s.Id,
					StudentCode = s.StudentCode,
					FullName = s.FullName,
					Gender = s.Gender,
					DateOfBirth = s.DateOfBirth,
					ClassId = s.ClassId,
					StudentClass = s.SchoolClass != null ? new SchoolClassResponse
					{
						Id = s.SchoolClass.Id,
						ClassName = s.SchoolClass.ClassName,
						ClassRoom = s.SchoolClass.ClassRoom,
						Quantity = s.SchoolClass.Quantity
					} : null,
					Image = s.Image,
					HealthProfile = s.HealthProfiles
						.Where(hp => hp.DeletedTime == null)
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
						}).FirstOrDefault(),
					HealthCheckupRecords = s.HealthCheckupRecords
						.Where(hcr => hcr.DeletedTime == null)
						.Select(hcr => new HealthCheckUpResponse
						{
							HealthActivityId = hcr.HealthActivityId,
							StudentId = hcr.StudentId,
							StudentName = hcr.Student.FullName,
							NurseId = hcr.LastUpdatedBy,
							Vision = hcr.Vision,
							Hearing = hcr.Hearing,
							Dental = hcr.Dental,
							BMI = hcr.BMI,
							AbnormalNote = hcr.AbnormalNote,
							Time = hcr.RecordDate,
							RecordDate = hcr.RecordDate,
							IsLatest = hcr.IsLatest,
							CheckingStatus = hcr.CheckingStatus
						}).ToList()
				}).FirstOrDefaultAsync();

			return student;
		}
	}
}
