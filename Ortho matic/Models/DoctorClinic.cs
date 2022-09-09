using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ortho_matic.Models
{
    public class DoctorClinic
    {
        public DoctorClinic()
        {
            Times = new HashSet<Time>();
        }

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }
        public int ClinicId { get; set; }
        [ForeignKey("ClinicId")]
        public virtual Clinic Clinic { get; set; }
        [MinLength(1)]
        public virtual ICollection<Time> Times { get; set; }
        public Time BestTimeForVisit { get; set; }
    }
}
