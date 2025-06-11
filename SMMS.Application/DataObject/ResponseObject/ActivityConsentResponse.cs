

using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
	public class ActivityConsentResponse
	{
		public string? Id { get; set; }
		public string? StudentId { get; set; }
		public string? StudentName { get; set; }
		public string? ActivityType { get; set; }
		public string? ActivityId { get; set; }
		public string? ActivityName { get; set; }
		public ApprovalStatus Status { get; set; }
		public DateTime ScheduleTime { get; set; }
		public string? ResponsibleUserId { get; set; }
		public string? ResponsibleUserName { get; set; }

	}
}
