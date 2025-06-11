namespace SMMS.Application.DataObject.ResponseObject
{
    public class ListMedicalRequestResponse
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string MedicalName { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Dosage { get; set; }
        public bool IsCompletedToday { get; set; }
        public DateTime? LastCompletedDate { get; set; }
    }
}
