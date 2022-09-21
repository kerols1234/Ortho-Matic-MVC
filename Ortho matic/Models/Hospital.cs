using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class Hospital
    {
        public Hospital()
        {
            DoctorHospitals = new HashSet<DoctorHospital>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual ICollection<DoctorHospital> DoctorHospitals { get; set; }
        public int RegionId { get; set; }
        [ForeignKey("RegionId")]
        public virtual Region Region { get; set; }
    }
}
