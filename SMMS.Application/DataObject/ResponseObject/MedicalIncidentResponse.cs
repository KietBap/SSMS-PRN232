using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
    public class MedicalIncidentResponse
    {
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }  
        public string Description { get; set; }
        public MedicalIncidentStatus Status { get; set; }
        public DateTime IncidentDate { get; set; }
        public List<ListMedicalUsageResponse> MedicalUsages { get; set; }
    }
}
