using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.RequestObject
{
    public class UpdateMedicalIncidentRequest
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public MedicalIncidentStatus Status { get; set; }
        public DateTime IncidentDate { get; set; }
    }
}
