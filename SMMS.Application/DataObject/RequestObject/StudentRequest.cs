

using Microsoft.AspNetCore.Http;

namespace SMMS.Application.DataObject.RequestObject
{
	public class StudentRequest
	{
		public string? FullName { get; set; }
		public string? Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? ClassId { get; set; }
		public IFormFile? Image { get; set; }
	}
}
