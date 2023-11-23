using System.ComponentModel.DataAnnotations;

namespace Practise101.Api.Models
{
    public class UserRegisterReguest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(1)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;    
    }
}
