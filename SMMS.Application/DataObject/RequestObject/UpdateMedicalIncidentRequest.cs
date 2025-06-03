namespace SMMS.Application.DataObject.RequestObject
{
    public class UpdateMedicalIncidentRequest
    {
        public string Type { get; set; }  
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime IncidentDate { get; set; }
    }
}
