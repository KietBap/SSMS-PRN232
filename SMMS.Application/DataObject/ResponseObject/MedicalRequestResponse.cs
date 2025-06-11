namespace SMMS.Application.DataObject.ResponseObject
{
    public class MedicalRequestResponse
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string UserId { get; set; }
        public string NurseName { get; set; }
        public string MedicalName { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTime? LastCompletedDate { get; set; }
        public bool IsCompletedToday { get; set; }
    }
}
