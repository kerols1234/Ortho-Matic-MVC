using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class Visitation
    {
        public int Id { get; set; }
        public string comment { get; set; }
        public DateTime TimeOfVisit { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public int? DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
        public int? HospitalId { get; set; }
        [ForeignKey("HospitalId")]
        public virtual Hospital Hospital { get; set; }
        public int? ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }
    }
}
