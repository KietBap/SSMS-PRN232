

namespace SMMS.Application.DataObject.ResponseObject
{
	public class SchoolClassResponse
	{
		public string? Id { get; set; }
		public string? ClassName { get; set; }
		public string? ClassRoom { get; set; }
		public int Quantity { get; set; }
		public List<StudentResponse> Students { get; set; } = new List<StudentResponse>(); // Danh sách sinh viên
	}
}
