

using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
	public class HealthCheckUpResponse
	{
		public string? HealthCheckUpId { get; set; }
		public string? HealthActivityId { get; set; }
		public string? StudentId { get; set; }
		public string? StudentName { get; set; }
		public string? NurseId { get; set; }
		public string? NurseName { get; set; }
		public string? Vision { get; set; }
		public string? Hearing { get; set; }
		public string? Dental { get; set; }
		public double? BMI { get; set; }
		public string? AbnormalNote { get; set; }
		public DateTime Time { get; set; }
		public DateTime RecordDate { get; set; }
		public bool IsLatest { get; set; }
		public CheckingStatus CheckingStatus { get; set; }
	}
}
