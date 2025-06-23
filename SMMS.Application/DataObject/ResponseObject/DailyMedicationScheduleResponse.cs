namespace SMMS.Application.DataObject.ResponseObject
{
    public class DailyMedicationScheduleResponse
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
        public string MedicationName { get; set; }
        public string Form { get; set; }
        public string Dosage { get; set; }
        public string Route { get; set; }
        public List<string> ScheduledTimes { get; set; }
        public string? Notes { get; set; }
        
        // Trạng thái hôm nay
        public List<DailyAdministrationStatus> TodayAdministrations { get; set; }
    }

    public class DailyAdministrationStatus
    {
        public string? AdministrationId { get; set; } // ID để cập nhật bản ghi
        public string ScheduledTime { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? DoseGiven { get; set; }
        public bool? WasTaken { get; set; }
        public string? Notes { get; set; }
        public string? AdministratorName { get; set; }
    }
}
