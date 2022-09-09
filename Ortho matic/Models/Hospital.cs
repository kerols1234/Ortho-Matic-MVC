using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual ICollection<DoctorHospital> DoctorHospitals { get; set; }
    }
}
