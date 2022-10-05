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
        public Specialty DoctorSpecialty { get; set; }
        public Degree DoctorDegree { get; set; }
        public virtual ICollection<DoctorHospital> DoctorHospitals { get; set; }
        public virtual ICollection<DoctorClinic> DoctorClinics { get; set; }
    }

    public enum Degree
    {
        A, B, C
    }

    public enum Specialty
    {
        Orthopedic,
        Surgeon,
        Physiotherapist
    }
}
