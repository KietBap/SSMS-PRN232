using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
