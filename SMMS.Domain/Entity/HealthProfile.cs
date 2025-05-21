using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class HealthProfile : BaseEntity
    {
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        public string Vision { get; set; }
        public string Hearing { get; set; }
        public string Dental { get; set; }
        public double BMI { get; set; }
        public string? AbnormalNote { get; set; }
        public string? VaccinationHistory { get; set; }
    }
}
