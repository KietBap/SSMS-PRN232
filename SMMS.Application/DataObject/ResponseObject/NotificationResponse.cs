

namespace SMMS.Application.DataObject.ResponseObject
{
	public class NotificationResponse
	{
		public string? Id { get; set; }
		public string? Title { get; set; }
		public string? Message { get; set; }
		public bool IsRead { get; set; }
		public DateTimeOffset CreatedTime { get; set; }

		public string EventId { get; set; } = string.Empty;
	}
}
