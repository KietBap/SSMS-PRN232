

namespace SMMS.Application.DataObject.RequestObject
{
	public class HealthActivityRequest
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public DateTime ScheduledDate { get; set; }
		public required List<string?> ClassIds { get; set; }
	}
}
