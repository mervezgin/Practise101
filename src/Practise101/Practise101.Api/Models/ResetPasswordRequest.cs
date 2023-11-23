using System.ComponentModel.DataAnnotations;

namespace Practise101.Api.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required, MinLength(1, ErrorMessage = "Please enter at least 1 character.")]
        public string Password { get; set; } = string.Empty;
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
