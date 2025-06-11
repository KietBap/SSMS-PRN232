

namespace SMMS.Application.DataObject.ResponseObject
{
	public class HealthProfileResponse
	{
		public string? Id { get; set; }
		public string? StudentId { get; set; }
		public string? Vision { get; set; }
		public string? Hearing { get; set; }
		public string? Dental { get; set; }
		public double BMI { get; set; }
		public string? AbnormalNote { get; set; }
		public string? VaccinationHistory { get; set; }
	}
}
