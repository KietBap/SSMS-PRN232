namespace SMMS.Application.DataObject.RequestObject
{
    public class CreateMedicalUsageRequest
    {
        public string MedicalIncidentId { get; set; }
        public List<MedicalUsageDetail> MedicalUsageDetails { get; set;}
    }
}
