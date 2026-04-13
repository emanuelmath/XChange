using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        [Required]
        public required string ConfirmPassword { get; set; }
    }
}
