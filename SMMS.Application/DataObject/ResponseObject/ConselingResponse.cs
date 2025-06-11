

namespace SMMS.Application.DataObject.ResponseObject
{
	public class ConselingResponse
	{
		public string? StudentId { get; set; }
		public string? StudentName { get; set; }
		public string? ParentName { get; set; }
		public string? HealthCheckupId { get; set; }
		public DateTime MeetingDate { get; set; }
		public string? Note { get; set; }
		public bool Status { get; set; }
		public DateTimeOffset CreatedTime { get; set; }
		public string? CreatedBy { get; set; }
		public DateTimeOffset? UpdatedTime { get; set; }
		public string? UpdatedBy { get; set; }
	}
}
