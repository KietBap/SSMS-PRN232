using SMMS.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Domain.Entity
{
    public class Otp : BaseEntity
    {
        public string PhoneNumber { get; set; }
        public string OtpCode { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public bool IsUsed { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}