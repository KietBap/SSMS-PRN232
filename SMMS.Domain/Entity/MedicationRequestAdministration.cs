using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class MedicationRequestAdministration : BaseEntity
    {
        [Required]
        public string MedicalRequestId { get; set; }

        [ForeignKey("MedicalRequestId")]
        public virtual MedicalRequest MedicalRequest { get; set; }

        public string? AdministeredBy { get; set; } // ID của y tá (null khi chưa thực hiện)

        [ForeignKey("AdministeredBy")]
        public virtual User? Administrator { get; set; }

        public DateTime AdministeredAt { get; set; } // Ngày giờ cho thuốc

        public string? DoseGiven { get; set; } // Liều lượng thực tế: "2 viên", "10ml" (null khi chưa thực hiện)

        public string? Notes { get; set; } // Ghi chú: trẻ không chịu uống, nôn thuốc, v.v.

        public bool WasTaken { get; set; } // Đã uống hay không
    }
}
