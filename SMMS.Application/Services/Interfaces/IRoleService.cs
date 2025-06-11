using SMMS.Application.DataObject.RequestObject;
using SMMS.Application.DataObject.ResponseObject;

namespace SMMS.Application.Services.Interfaces
{
	public interface IRoleService
	{
		Task<List<RoleResponse>> GetAllRolesAsync();
		Task<RoleResponse> GetRoleByIdAsync(string id);
		Task<bool> CreateRoleAsync(RoleRequest request);
		Task<bool> UpdateRoleAsync(string id, RoleRequest request);
		Task<bool> DeleteRoleAsync(string id);
	}
}
