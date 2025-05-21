using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class VaccinationCampaign : BaseEntity
    {
        public string Name { get; set; }
        public string VaccineName { get; set; }
        public DateTime EXP { get; set; }
        public DateTime MFG { get; set; }
        public string VaccineType { get; set; }
        public DateTime StartDate { get; set; }
        public virtual ICollection<VaccinationRecord> VaccinationRecords { get; set; }
        public virtual ICollection<ActivityConsent> ActivityConsents { get; set; }
    }
}
