using ClosedXML.Excel;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;



namespace SMMS.Application.Helpers.Implements
{
	public class ImportService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ImportService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task ImportStudentsFromExcelAsync(Stream fileStream)
		{
			using var workbook = new XLWorkbook(fileStream);
			var worksheet = workbook.Worksheet(1); // Giả sử dữ liệu nằm ở sheet đầu tiên

			var rows = worksheet.RowsUsed().Skip(1); // Bỏ qua hàng tiêu đề

			foreach (var row in rows)
			{
				var parentEmail = row.Cell(1).GetString();
				var parentPhone = row.Cell(2).GetString();
				var parentFullName = row.Cell(3).GetString();
				var studentFullName = row.Cell(4).GetString();
				var studentGender = row.Cell(5).GetString();
				var studentDateOfBirth = row.Cell(6).GetDateTime();
				var className = row.Cell(7).GetString();

				// Tìm hoặc tạo SchoolClass
				var schoolClass = _repositoryManager.ClassRepository
					.FindByCondition(c => c.ClassName == className, true)
					.FirstOrDefault();
				if (schoolClass == null)
				{
					schoolClass = new SchoolClass
					{
						Id = Guid.NewGuid().ToString(),
						ClassName = className,
						ClassRoom = className,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					};
					_repositoryManager.ClassRepository.Create(schoolClass);
				}

				var parentUser = _repositoryManager.UserRepository
					.FindByCondition(u => u.FullName == parentFullName && u.Email == parentEmail, true)
					.FirstOrDefault();

				var student = _repositoryManager.StudentRepository
				.FindByCondition(s => s.FullName == studentFullName, true)
				.FirstOrDefault();

				User? oldParent = null;

				if(student != null)
				{
					oldParent = _repositoryManager.UserRepository
						.FindByCondition(u => u.Id == student.ParentId, true)
						.FirstOrDefault();
				}

				if (parentUser == null)
				{
					// Case: Parent is completely new
					var parentRole = _repositoryManager.RoleRepository
						.FindByCondition(r => r.RoleName == "Parent", false)
						.FirstOrDefault();
					if (parentRole == null)
					{
						throw new Exception("Role 'Parent' not found.");
					}

					parentUser = new User
					{
						Email = parentEmail,
						Phone = parentPhone,
						FullName = parentFullName,
						RoleId = parentRole.Id,
						Password = BCrypt.Net.BCrypt.HashPassword("123456"), // Mật khẩu mặc định
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					};
					_repositoryManager.UserRepository.Create(parentUser);
				}
				else if (parentUser.DeletedBy != null && parentUser.DeletedTime != null)
				{
					// Case: Parent exists but is soft-deleted
					parentUser.DeletedTime = null;
					parentUser.DeletedBy = null;
					parentUser.Phone = parentPhone;
					parentUser.Email = parentEmail;
					parentUser.LastUpdatedBy = "System";
					parentUser.LastUpdatedTime = DateTimeOffset.UtcNow;
					_repositoryManager.UserRepository.Update(parentUser);
				}
				else if (parentUser.Phone != parentPhone || parentUser.FullName != parentFullName || parentUser.Email != parentEmail )
				{
					// Case: Parent exists, not soft-deleted, but phone or full name differs
					parentUser.Phone = parentPhone;
					parentUser.Email = parentEmail;
					parentUser.LastUpdatedBy = "System";
					parentUser.LastUpdatedTime = DateTimeOffset.UtcNow;
					_repositoryManager.UserRepository.Update(parentUser);
				}

				if (student == null)
				{
					// Case: Student is new
					student = new Student
					{
						ParentId = parentUser.Id,
						ClassId = schoolClass.Id,
						FullName = studentFullName,
						Gender = studentGender,
						DateOfBirth = studentDateOfBirth,
						CreatedBy = "System",
						CreatedTime = DateTimeOffset.UtcNow
					};
					_repositoryManager.StudentRepository.Create(student);
				}
				else
				{
					// Case: Student exists, check for updates
					bool hasChanges = false;
					if (student.ClassId != schoolClass.Id ||
						student.Gender != studentGender ||
						student.DateOfBirth != studentDateOfBirth ||
						student.ParentId != parentUser.Id)
					{
						hasChanges = true;
						student.ClassId = schoolClass.Id;
						student.Gender = studentGender;
						student.DateOfBirth = studentDateOfBirth;
						student.ParentId = parentUser.Id;
						student.LastUpdatedBy = "System";
						student.LastUpdatedTime = DateTimeOffset.UtcNow;
						//_repositoryManager.StudentRepository.Update(student);
					}
					// Handle old parent soft-deletion if parent has changed and old parent exists
					if (hasChanges && oldParent != null && oldParent.Id != parentUser.Id)
					{
						// Check if old parent is still associated with other students
						var otherStudents = _repositoryManager.StudentRepository
							.FindByCondition(s => s.ParentId == oldParent.Id && s.Id != student.Id, false)
							.Any();
						if (!otherStudents)
						{
							// Soft-delete old parent if not linked to other students
							oldParent.DeletedBy = "System";
							oldParent.DeletedTime = DateTimeOffset.UtcNow;
							_repositoryManager.UserRepository.Update(oldParent);
						}
					}
				}
			}
			await _repositoryManager.SaveAsync();
		}
	}
}
