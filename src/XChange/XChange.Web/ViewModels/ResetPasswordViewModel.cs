using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set;  }

        [Required]
        public required string NewPassword { get; set; }

    }
}
