using System.ComponentModel.DataAnnotations;

namespace SMMS.Application.DataObject.RequestObject
{
    public class UpdateMedicalRequestRequest
    {
        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string Form { get; set; }

        [Required]
        public string Dosage { get; set; }

        [Required]
        public string Route { get; set; }

        [Required]
        public int Frequency { get; set; }

        [Required]
        public int TotalQuantity { get; set; }

        [Required]
        public List<string> TimeToAdminister { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Notes { get; set; }
    }
}
