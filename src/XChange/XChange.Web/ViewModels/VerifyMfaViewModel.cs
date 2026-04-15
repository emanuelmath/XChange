using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class VerifyMfaViewModel
    {
        [Required]
        public required string Code { get; set; }
    }
}
