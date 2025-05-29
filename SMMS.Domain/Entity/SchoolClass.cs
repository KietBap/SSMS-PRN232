using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class SchoolClass : BaseEntity
    {
        public string ClassName { get; set; }
        public string ClassRoom { get; set; }
        public int Quantity { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
