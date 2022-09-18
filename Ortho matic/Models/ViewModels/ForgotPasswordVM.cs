using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
