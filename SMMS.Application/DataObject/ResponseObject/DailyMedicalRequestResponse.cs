namespace SMMS.Application.DataObject.ResponseObject
{
    public class DailyMedicalRequestResponse
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string MedicalName { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
