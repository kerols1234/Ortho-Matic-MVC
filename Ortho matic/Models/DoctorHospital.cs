using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class DoctorHospital
    {
        public DoctorHospital()
        {
            Times = new HashSet<Time>();
        }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
        public int HospitalId { get; set; }
        [ForeignKey("HospitalId")]
        public virtual Hospital Hospital { get; set; }
        [MinLength(1)]
        public virtual ICollection<Time> Times { get; set; }
        public Time BestTimeForVisit { get; set; }
        public DateTime LastTimeOfVisitation { get; set; } = DateTime.Now.Subtract(TimeSpan.FromHours(49));
    }
}
