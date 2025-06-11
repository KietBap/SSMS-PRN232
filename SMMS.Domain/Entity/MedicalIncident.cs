using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SMMS.Domain.Enum;

namespace SMMS.Domain.Entity
{
    public class MedicalIncident : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string Type { get; set; }
        public string Description { get; set; }
        public MedicalIncidentStatus Status { get; set; }
        public DateTime IncidentDate { get; set; }
        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
