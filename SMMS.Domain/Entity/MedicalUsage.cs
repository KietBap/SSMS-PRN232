using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class MedicalUsage : BaseEntity
    {
        [Required]
        public string MedicalIncidentId { get; set; }

        [ForeignKey("MedicalIncidentId")]
        public virtual MedicalIncident MedicalIncident { get; set; }

        [Required]
        public string MedicalStockId { get; set; }

        [ForeignKey("MedicalStockId")]
        public virtual MedicalStock MedicalStock { get; set; }

        public string MedicalName { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
