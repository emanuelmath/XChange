using System.ComponentModel.DataAnnotations;

namespace XChange.Web.ViewModels
{
    public class VerifyEmailViewModel
    {

        [Required]
        public required string Code { get; set; }
    }
}
