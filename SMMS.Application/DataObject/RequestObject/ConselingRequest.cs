
namespace SMMS.Application.DataObject.RequestObject
{
	public class ConselingRequest
	{
		public string? StudentId { get; set; }
		public string? HealthCheckupId { get; set; }
		public string? Note { get; set; }
		public DateTime RequestedDate { get; set; }
	}
}
