using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
