using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Employee Name")]
        [Required]
        public string EmployeeName { get; set; }


    }
}
