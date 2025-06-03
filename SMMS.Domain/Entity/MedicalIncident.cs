using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        public string Type { get; set; }  //Pending, Handled, Cancelled 
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime IncidentDate { get; set; }
        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
