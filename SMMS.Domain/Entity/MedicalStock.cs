using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class MedicalStock : BaseEntity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DetailInformation { get; set; }
        public string Status { get; set; }

        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
