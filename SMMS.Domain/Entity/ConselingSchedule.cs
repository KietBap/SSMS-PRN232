using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SMMS.Domain.Enum;

namespace SMMS.Domain.Entity
{
    public class ConselingSchedule : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual User Parent { get; set; }

        [Required]
        public string MedicalStaffId { get; set; }

        [ForeignKey("MedicalStaffId")]
        public virtual User MedicalStaff { get; set; }

        [Required]
        public string HealthCheckupId { get; set; }

        [ForeignKey("HealthCheckupId")]
        public virtual HealthCheckupRecord HealthCheckupRecord { get; set; }

        public string Note { get; set; }
        public DateTime MeetingDate { get; set; }
        public ApprovalStatus Status { get; set; }
    }
}
