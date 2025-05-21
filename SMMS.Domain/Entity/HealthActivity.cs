using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class HealthActivity : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public virtual ICollection<HealthCheckupRecord> HealthCheckupRecords { get; set; }
        public virtual ICollection<ActivityConsent> ActivityConsents { get; set; }
    }
}
