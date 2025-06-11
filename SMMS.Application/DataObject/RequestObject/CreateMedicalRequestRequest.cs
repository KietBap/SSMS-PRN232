namespace SMMS.Application.DataObject.RequestObject
{
    public class CreateMedicalRequestRequest
    {
        public string StudentId { get; set; }
        public string ParentId { get; set; }
        public string MedicalName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public string? Notes { get; set; }
    }
}
