

namespace SMMS.Application.DataObject.ResponseObject
{
	public class ParentResponse
	{
		public string? Id { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? FullName { get; set; }
		public string? RoleName { get; set; }
		public string? ImageUrl { get; set; }

		public List<StudentResponse>? Students { get; set; }
	}
}
