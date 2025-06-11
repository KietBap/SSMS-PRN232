

using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
	public class VaccinationCampaignResponse
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? VaccineName { get; set; }
		public string? NurseId { get; set; }
		public string? NurseName { get; set; }
		public DateTime EXP { get; set; }
		public DateTime MFG { get; set; }
		public string? VaccineType { get; set; }
		public DateTime StartDate { get; set; }
		public ApprovalStatus Status { get; set; }
		public List<string> ClassIds { get; set; }
	}
}
