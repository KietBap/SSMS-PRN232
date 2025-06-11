

using SMMS.Domain.Entity;

namespace SMMS.Application.DataObject.ResponseObject
{
	public class StudentHealthResponse
	{
		public string? Id { get; set; }
		public string? FullName { get; set; }
		public string? Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? ClassId { get; set; }
		public SchoolClass? StudentClass { get; set; }
		public string? Image { get; set; }
		public HealthProfileResponse? HealthProfile { get; set; }
	}
}
