using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class ActivityConsent : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public string VaccinationCampaignId { get; set; }

        [ForeignKey("VaccinationCampaignId")]
        public virtual VaccinationCampaign VaccinationCampaign { get; set; }

        [Required]
        public string HealthActivityId { get; set; }

        [ForeignKey("HealthActivityId")]
        public virtual HealthActivity HealthActivity { get; set; }

        public string Comments { get; set; }
        public bool Status { get; set; } 
        public DateTime ScheduleTime { get; set; }
        public string ActivityType { get; set; }
    }
}
