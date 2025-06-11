using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SMMS.Domain.Enum;

namespace SMMS.Domain.Entity
{
    public class VaccinationCampaign : BaseEntity
    {
		[Required]
		public string UserId { get; set; }

		[ForeignKey("UserId")]
		public virtual User User { get; set; }
		public string Name { get; set; }
        public string VaccineName { get; set; }
        public DateTime EXP { get; set; }
        public DateTime MFG { get; set; }
        public string VaccineType { get; set; }
        public DateTime StartDate { get; set; }
		public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending; // Thay IsAccepted

		public virtual ICollection<VaccinationCampaignClass> VaccinationCampaignClasses { get; set; }
		public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
        public virtual ICollection<ActivityConsent> ActivityConsents { get; set; }
    }
}
