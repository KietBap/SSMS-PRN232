using SMMS.Domain.Base;

namespace SMMS.Domain.Entity
{
    public class Document : BaseEntity
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
    }
}
