namespace SMMS.Application.DataObject.ResponseObject
{
    public class ListMedicalIncidentResponse
    {
        public string Id { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime IncidentDate { get; set; }
    }
}
