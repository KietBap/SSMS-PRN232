using SMMS.Domain.Enum;

namespace SMMS.Application.DataObject.ResponseObject
{
    public class ListMedicalStockResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string DetailInformation { get; set; }
        public MedicalStockStatus Status { get; set; }
    }
}
