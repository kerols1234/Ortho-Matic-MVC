using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models
{
    public class VisitTask
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required]
        public string comment { get; set; }
        public int DoctorId { get; set; }
        public int? HospitalId { get; set; }

        public int? ClinicId { get; set; }
    }
}
