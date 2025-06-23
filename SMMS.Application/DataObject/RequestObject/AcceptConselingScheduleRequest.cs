
using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.RequestObject
{
	public class AcceptConselingScheduleRequest
	{
		public string? ConselingScheduleId { get; set; }
		public ApprovalStatus Status { get; set; }
	}
}
