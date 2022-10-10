using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class Clinic
    {
        public Clinic()
        {
            DoctorClinics = new HashSet<DoctorClinic>();
        }

        public int Id { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [Display(Name = "phone number 1")]
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string Phone1 { get; set; }
        [Display(Name = "phone number 2")]
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string Phone2 { get; set; }
        [Display(Name = "phone number 3")]
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string Phone3 { get; set; }
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; }
        public int? RegionId { get; set; }
        [ForeignKey("RegionId")]
        [Display(Name = "Area")]
        public virtual Region Region { get; set; }
    }
}
