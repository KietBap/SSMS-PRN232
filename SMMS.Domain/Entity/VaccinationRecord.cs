using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class VaccinationRecord : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string VaccinationCampaignId { get; set; }

        [ForeignKey("VaccinationCampaignId")]
        public virtual VaccinationCampaign VaccinationCampaign { get; set; }
        public string ResultNote { get; set; }
        public DateTime Time { get; set; }
		public DateTime VaccinatedAt { get; set; }
    }
}
