

namespace SMMS.Application.DataObject.RequestObject
{
	public class HealthProfileRequest
	{
		public string? Vision { get; set; }
		public string? Hearing { get; set; }
		public string? Dental { get; set; }
		public double BMI { get; set; }
		public string? AbnormalNote { get; set; }
		public string? VaccinationHistory { get; set; }
	}
}
