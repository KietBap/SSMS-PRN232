namespace SMMS.Application.DataObject.ResponseObject
{
    public class CompletedMedicationHistoryResponse
    {
        public string Id { get; set; } // Administration ID
        public string MedicalRequestId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
        public string MedicationName { get; set; }
        public string Form { get; set; }
        public string Route { get; set; }
        public string PlannedDosage { get; set; } // Liều lượng dự kiến từ MedicalRequest
        public string ActualDoseGiven { get; set; } // Liều lượng thực tế đã cho
        public DateTime ScheduledTime { get; set; } // Thời gian dự kiến
        public DateTime CompletedAt { get; set; } // Thời gian thực tế hoàn thành
        public bool WasTaken { get; set; } // Đã uống hay không
        public string? Notes { get; set; } // Ghi chú
        public string AdministratorId { get; set; }
        public string AdministratorName { get; set; }
        public string Status { get; set; } // "Success" hoặc "Failed"
        public TimeSpan? TimeDifference { get; set; } // Chênh lệch thời gian so với lịch trình
    }

    public class DailyCompletedMedicationSummary
    {
        public DateTime Date { get; set; }
        public int TotalScheduled { get; set; } // Tổng số lịch trong ngày
        public int TotalCompleted { get; set; } // Tổng số đã thực hiện
        public int SuccessfulAdministrations { get; set; } // Số lần uống thành công
        public int FailedAdministrations { get; set; } // Số lần không uống được
        public double SuccessRate { get; set; } // Tỷ lệ thành công (%)
        public List<CompletedMedicationHistoryResponse> CompletedAdministrations { get; set; }
    }
}
