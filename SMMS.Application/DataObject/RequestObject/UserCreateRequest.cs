

namespace SMMS.Application.DataObject.RequestObject
{
	public class UserCreateRequest
	{
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? FullName { get; set; }
		public string? RoleId { get; set; }
		public string? Password { get; set; }
	}
}
