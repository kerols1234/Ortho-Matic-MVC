using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models
{
    public class Doctor
    {
        public Doctor()
        {
            DoctorHospitals = new HashSet<DoctorHospital>();
            DoctorClinics = new HashSet<DoctorClinic>();
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Specialty { get; set; }
        public Degree DoctorDegree { get; set; }
        [StringLength(11, ErrorMessage = "Invalid Mobile Number", MinimumLength = 11)]
        [RegularExpression(@"^01([0-9]{9})", ErrorMessage = "Invalid Mobile Number")]
        public string PhoneNumber { get; set; }
        public virtual ICollection<DoctorHospital> DoctorHospitals { get; set; }
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; }
    }

    public enum Degree
    {
        A, B, C
    }
}
