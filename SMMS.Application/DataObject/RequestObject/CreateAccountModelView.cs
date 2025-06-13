using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SMMS.Application.DataObject.RequestObject
{
    public class CreateAccountModelView
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string VerifyPassword { get; set; }

        [Required]
        public string FullName { get; set; }

        public IFormFile? Image { get; set; }

        public string IdToken { get; set; }
    }
}
