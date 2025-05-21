using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class Student : BaseEntity
    {
        [Required]
        public string ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual User Parent { get; set; }

        [Required]
        public string ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Image { get; set; }
        public virtual ICollection<HealthProfile> HealthProfiles { get; set; }
        public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
        public virtual ICollection<ActivityConsent> ActivityConsents { get; set; }
        public virtual ICollection<MedicalIncident> MedicalIncidents { get; set; }
        public virtual ICollection<MedicalRequest> MedicalRequests { get; set; }
        public virtual ICollection<HealthCheckupRecord> HealthCheckupRecords { get; set; }
        public virtual ICollection<ConselingSchedule> ConselingSchedules { get; set; }
    }
}
