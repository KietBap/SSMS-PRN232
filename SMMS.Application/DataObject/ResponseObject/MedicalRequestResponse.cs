namespace SMMS.Application.DataObject.ResponseObject
{
    public class MedicalRequestResponse
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentClass { get; set; }
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string NurseName { get; set; }
        
        // Thông tin thuốc
        public string MedicationName { get; set; }
        public string Form { get; set; }
        public string Dosage { get; set; }
        public string Route { get; set; }
        public int Frequency { get; set; }
        
        // Số lượng
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        
        // Thời gian
        public List<string> TimeToAdminister { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public string? Notes { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        
        // Lịch sử cho thuốc
        public List<MedicationAdministrationResponse> Administrations { get; set; }
    }

    public class MedicationAdministrationResponse
    {
        public string Id { get; set; }
        public string AdministeredBy { get; set; }
        public string AdministratorName { get; set; }
        public DateTime AdministeredAt { get; set; }
        public string DoseGiven { get; set; }
        public bool WasTaken { get; set; }
        public string? Notes { get; set; }
    }
}
