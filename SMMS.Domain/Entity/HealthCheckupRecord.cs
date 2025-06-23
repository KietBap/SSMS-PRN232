using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SMMS.Domain.Enum;

namespace SMMS.Domain.Entity
{
    public class HealthCheckupRecord : BaseEntity
    {
        [Required]
        public string HealthActivityId { get; set; }

        [ForeignKey("HealthActivityId")]
        public virtual HealthActivity HealthActivity { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public string Vision { get; set; }
        public string Hearing { get; set; }
        public string Dental { get; set; }
        public double BMI { get; set; }
        public string AbnormalNote { get; set; }
        public DateTime Time { get; set; }
		public DateTime RecordDate { get; set; }
        public bool IsLatest { get; set; }

        public CheckingStatus CheckingStatus { get; set; }

		public virtual ICollection<ConselingSchedule> ConselingSchedules { get; set; }
    }
}
