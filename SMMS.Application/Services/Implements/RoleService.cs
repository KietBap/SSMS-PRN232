using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;
using SMMS.Application.Services.Interfaces;
using SMMS.Domain.Entity;
using SMMS.Domain.Interface.Repositories;

namespace SMMS.Application.Services.Implements
{
	public class RoleService : IRoleService
	{
		private readonly IRepositoryManager _repositoryManager;

		public RoleService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<List<RoleResponse>> GetAllRolesAsync()
		{
			var roles = _repositoryManager.RoleRepository.FindAll(false)
				.Where(r => r.DeletedTime == null)
				.Select(r => new RoleResponse
				{
					Id = r.Id,
					RoleName = r.RoleName
				}).ToList();
			return roles;
		}

		public async Task<RoleResponse> GetRoleByIdAsync(string id)
		{
			var role = _repositoryManager.RoleRepository.FindByCondition(r => r.Id == id && r.DeletedTime == null, false)
				.Select(r => new RoleResponse
				{
					Id = r.Id,
					RoleName = r.RoleName
				}).FirstOrDefault();
			return role;
		}

		public async Task<bool> CreateRoleAsync(RoleRequest request)
		{
			if (string.IsNullOrEmpty(request.RoleName))
			{
				throw new ArgumentException("RoleName cannot be null or empty.", nameof(request.RoleName));
			}

			var role = new Role
			{
				RoleName = request.RoleName,
				CreatedBy = "Admin", // Could be replaced with current user ID
				CreatedTime = DateTimeOffset.UtcNow
			};
			_repositoryManager.RoleRepository.Create(role);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> UpdateRoleAsync(string id, RoleRequest request)
		{
			if (string.IsNullOrEmpty(request.RoleName))
			{
				throw new ArgumentException("RoleName cannot be null or empty.", nameof(request.RoleName));
			}

			var role = _repositoryManager.RoleRepository.FindByCondition(r => r.Id == id && r.DeletedTime == null, true)
				.FirstOrDefault();
			if (role == null) return false;

			role.RoleName = request.RoleName;
			role.LastUpdatedBy = "Admin"; // Could be replaced with current user ID
			role.LastUpdatedTime = DateTimeOffset.UtcNow;

			_repositoryManager.RoleRepository.Update(role);
			await _repositoryManager.SaveAsync();
			return true;
		}

		public async Task<bool> DeleteRoleAsync(string id)
		{
			var role = _repositoryManager.RoleRepository.FindByCondition(r => r.Id == id && r.DeletedTime == null, true)
				.FirstOrDefault();
			if (role == null) return false;

			role.DeletedBy = "Admin"; // Could be replaced with current user ID
			role.DeletedTime = DateTimeOffset.UtcNow;
			_repositoryManager.RoleRepository.Update(role);
			await _repositoryManager.SaveAsync();
			return true;
		}
	}
}
