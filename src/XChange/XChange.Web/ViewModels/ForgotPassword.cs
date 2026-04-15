using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
