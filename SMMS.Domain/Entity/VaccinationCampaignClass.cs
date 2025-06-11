using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
	public class VaccinationCampaignClass : BaseEntity
	{
		public string VaccinationCampaignId { get; set; }
		public string SchoolClassId { get; set; }

		public virtual VaccinationCampaign VaccinationCampaign { get; set; }
		public virtual SchoolClass SchoolClass { get; set; }
	}
}
