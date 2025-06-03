namespace SMMS.Application.DataObject.ResponseObject
{
    public class MedicalStockResponse
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DetailInformation { get; set; }
    }
}
