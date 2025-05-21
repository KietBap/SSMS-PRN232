using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class Class : BaseEntity
    {
        public string ClassName { get; set; }
        public string ClassRoom { get; set; }
        public int Quantity { get; set; }
        public virtual ICollection<Student> Students { get; set; }
    }
}
