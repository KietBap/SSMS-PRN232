using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SMMS.Domain.Enum;

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

        public string? VaccinationCampaignId { get; set; }

        [ForeignKey("VaccinationCampaignId")]
        public virtual VaccinationCampaign VaccinationCampaign { get; set; }
        public string? HealthActivityId { get; set; }

        [ForeignKey("HealthActivityId")]
        public virtual HealthActivity HealthActivity { get; set; }

        public string? Comments { get; set; }
		public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
		public DateTime ScheduleTime { get; set; }
        public string? ActivityType { get; set; }
    }
}
