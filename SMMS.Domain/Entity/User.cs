using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class User : BaseEntity
    {
        [Required]
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string? Image { get; set; }

        public virtual ICollection<Otp> Otps { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<ActivityConsent> ActivityConsents { get; set; }
        public virtual ICollection<HealthActivity> HealthActivities { get; set; }
        public virtual ICollection<MedicalIncident> MedicalIncidents { get; set; }
        public virtual ICollection<MedicalRequest> MedicalRequests { get; set; }
        public virtual ICollection<ConselingSchedule> ParentConselingSchedules { get; set; } // Dành cho Parent
        public virtual ICollection<ConselingSchedule> StaffConselingSchedules { get; set; } // Dành cho MedicalStaff
    }
}
