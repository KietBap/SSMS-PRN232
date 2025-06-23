namespace SMMS.Application.DataObject.ResponseObject
{
    public class ListMedicalRequestResponse
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
        public string ParentName { get; set; }
        public string MedicationName { get; set; }
        public string Form { get; set; }
        public string Dosage { get; set; }
        public int Frequency { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public List<string> TimeToAdminister { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        
        // Thống kê cho thuốc
        public int TotalAdministrations { get; set; }
        public int CompletedAdministrations { get; set; }
        public DateTime? LastAdministeredAt { get; set; }
    }
}
