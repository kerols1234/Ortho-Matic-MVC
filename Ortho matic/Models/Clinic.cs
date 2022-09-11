using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; }
    }
}
