using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models.ViewModels
{
    public class UpdateVM
    {
        public string Id { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "User Name")]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(11, ErrorMessage = "The must be at least characters long.", MinimumLength = 11)]
        [Phone]
        public string PhoneNumber { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public int? RegionId { get; set; }
        public string Region { get; set; }
    }
}
