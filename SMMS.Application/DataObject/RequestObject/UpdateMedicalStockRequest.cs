namespace SMMS.Application.DataObject.RequestObject
{
    public class UpdateMedicalStockRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DetailInformation { get; set; }
        public string Status { get; set; }
    }
}
