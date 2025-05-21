using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class Blog : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string Title { get; set; }
        public string? Image { get; set; }
        public string Content { get; set; }
        public string? Excerpt { get; set; }
        public int View { get; set; }
    }
}
