using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class Notification : BaseEntity
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }

        public string EventId { get; set; }

	}
}
