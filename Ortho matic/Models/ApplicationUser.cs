using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Employee Name")]
        [Required]
        public string EmployeeName { get; set; }

        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
    }
}
