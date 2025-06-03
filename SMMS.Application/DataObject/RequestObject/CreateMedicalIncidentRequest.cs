namespace SMMS.Application.DataObject.RequestObject
{
    public class CreateMedicalIncidentRequest
    {
        public string StudentId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
    }
}
