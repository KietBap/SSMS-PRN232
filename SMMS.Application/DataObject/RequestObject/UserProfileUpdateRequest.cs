using Microsoft.AspNetCore.Http;


namespace SMMS.Application.DataObject.RequestObject
{
	public class UserProfileUpdateRequest
	{
		public string? FullName { get; set; }
		public string? Phone { get; set; }
		public IFormFile? Image { get; set; } 
	}
}
