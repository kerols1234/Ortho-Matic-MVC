using System.Collections.Generic;

namespace Ortho_matic.Models.APIModels
{
    public class ClinicTask
    {
        public ClinicTask()
        {
            Times = new HashSet<Time>();
        }

        public int ClinicId { get; set; }
        public string ClinicAddress { get; set; }
        public string ClinicPhone1 { get; set; }
        public string ClinicPhone2 { get; set; }
        public string ClinicPhone3 { get; set; }
        public ICollection<Time> Times { get; set; }
        public Time BestTimeForVisit { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public Specialty DoctorSpecialty { get; set; }
        public Degree DoctorDegree { get; set; }
    }
}
