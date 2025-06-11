

using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
	public class HealthActivityResponse
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime ScheduledDate { get; set; }
		public ApprovalStatus Status { get; set; }
		public List<string> ClassIds { get; set; }
	}
}
