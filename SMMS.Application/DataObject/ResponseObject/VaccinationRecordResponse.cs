

namespace SMMS.Application.DataObject.ResponseObject
{
	public class VaccinationRecordResponse
	{
		public string Id { get; set; }
		public string StudentId { get; set; }
		public string StudentName { get; set; }
		public string VaccinationCampaignId { get; set; }
		public string VaccineName { get; set; }
		public string ResultNote { get; set; }
		public DateTime Time { get; set; }
		public DateTime VaccinatedAt { get; set; }
	}
}
