using System.ComponentModel.DataAnnotations;

namespace SMMS.Application.DataObject.RequestObject
{
    public class RecordMedicationAdministrationRequest
    {
        public string? AdministrationId { get; set; } // ID của bản ghi administration đã có (nếu cập nhật)

        [Required]
        public string MedicalRequestId { get; set; }

        [Required]
        public string DoseGiven { get; set; } // Liều lượng thực tế: "2 viên", "10ml"

        [Required]
        public bool WasTaken { get; set; } // Đã uống hay không

        public DateTime? AdministeredAt { get; set; } // Thời gian thực tế cho thuốc (nếu khác với lịch trình)

        public string? Notes { get; set; } // Ghi chú: trẻ không chịu uống, nôn thuốc, v.v.
    }
}
