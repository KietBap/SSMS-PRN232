using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class MedicalRequest : BaseEntity
    {
        public MedicalRequest()
        {
            MedicationRequestAdministrations = new HashSet<MedicationRequestAdministration>();
        }
        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        [Required]
        public string ParentId { get; set; }

        [ForeignKey("ParentId")]
        public virtual User Parent { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Thông tin phụ huynh
        [Required]
        public string ParentName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        // Thông tin thuốc
        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string Form { get; set; } // Tablet, Syrup, EyeDrop, Cream

        [Required]
        public string Dosage { get; set; } // 2 viên/lần, 200ml/lần

        [Required]
        public string Route { get; set; } // Uống, chích, ngậm

        public int Frequency { get; set; } // 2 lần/ngày

        // Số lượng thuốc
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }

        // Thời gian cho thuốc
        [Required]
        public string TimeToAdminister { get; set; } // JSON string: ["07:00","11:00","19:00"]

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? Notes { get; set; }

        [Required]
        public string Status { get; set; } = "Active";

        // Navigation property
        public virtual ICollection<MedicationRequestAdministration> MedicationRequestAdministrations { get; set; }
    }
}
