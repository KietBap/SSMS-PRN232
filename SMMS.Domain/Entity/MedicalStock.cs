using SMMS.Domain.Base;
using SMMS.Domain.Enum;

namespace SMMS.Domain.Entity
{
    public class MedicalStock : BaseEntity
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DetailInformation { get; set; }
        public string Image { get; set; }
        public MedicalStockStatus Status { get; set; }

        public virtual ICollection<MedicalUsage> MedicalUsages { get; set; }
    }
}
