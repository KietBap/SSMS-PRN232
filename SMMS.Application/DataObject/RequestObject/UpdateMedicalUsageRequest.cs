namespace SMMS.Application.DataObject.RequestObject
{
    public class UpdateMedicalUsageRequest
    {
        public string MedicalStockId { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
