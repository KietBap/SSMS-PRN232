using System.ComponentModel.DataAnnotations;

namespace SMMS.Application.DataObject.RequestObject
{
    public class CreateMedicalRequestRequest
    {
        [Required]
        public string StudentId { get; set; }

        [Required]
        public string ParentId { get; set; }

        [Required]
        public List<MedicalRequestItem> MedicalRequestItems { get; set; }
    }

    public class MedicalRequestItem
    {
        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string Form { get; set; } // Tablet, Syrup, EyeDrop, Cream

        [Required]
        public string Dosage { get; set; } // 2 viên/lần, 200ml/lần

        [Required]
        public string Route { get; set; } // Uống, chích, ngậm

        [Required]
        public int Frequency { get; set; } // 2 lần/ngày

        [Required]
        public int TotalQuantity { get; set; }

        [Required]
        public List<string> TimeToAdminister { get; set; } // ["07:00","11:00","19:00"]

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Notes { get; set; }
    }
}
