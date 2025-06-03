namespace SMMS.Application.DataObject.ResponseObject
{
    public class ListMedicalUsageResponse
    {
        public string Id { get; set; }
        public string MedicalName { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
