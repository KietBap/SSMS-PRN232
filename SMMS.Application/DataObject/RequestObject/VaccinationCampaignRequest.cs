

namespace SMMS.Application.DataObject.RequestObject
{
	public class VaccinationCampaignRequest
	{
		public string? Name { get; set; }
		public string? VaccineName { get; set; }
		public DateTime EXP { get; set; }
		public DateTime MFG { get; set; }
		public string? VaccineType { get; set; }
		public DateTime StartDate { get; set; }
		public required List<string?> ClassIds { get; set; }
	}
}
