using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class Administrator
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "phone number")]
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }
        public string Specialty { get; set; }
        public string Comments { get; set; }
        public int? RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
        public DateTime LastTimeOfVisitation { get; set; } = DateTime.Now.Subtract(TimeSpan.FromHours(49));
    }
}
