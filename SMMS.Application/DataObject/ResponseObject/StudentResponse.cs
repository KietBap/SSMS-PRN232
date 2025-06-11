
namespace SMMS.Application.DataObject.ResponseObject
{
	public class StudentResponse
	{
		public string? Id { get; set; }
		public string? StudentCode { get; set; }
		public string? FullName { get; set; }
		public string? Gender { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string? ClassId { get; set; }
		public SchoolClassResponse? StudentClass { get; set; }
		public string? Image { get; set; }
		public HealthProfileResponse? HealthProfile { get; set; }
		public List<HealthCheckUpResponse>? HealthCheckupRecords { get; set; }
	}
}
