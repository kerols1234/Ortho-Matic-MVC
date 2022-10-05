using System.Collections.Generic;

namespace Ortho_matic.Models.APIModels
{
    public class HospitalTask
    {
        public HospitalTask()
        {
            Times = new HashSet<Time>();
        }

        public int HospitalId { get; set; }
        public string HospitalAddress { get; set; }
        public string HospitalPhone1 { get; set; }
        public string HospitalPhone2 { get; set; }
        public string HospitalPhone3 { get; set; }
        public string HospitalName { get; set; }
        public ICollection<Time> Times { get; set; }
        public Time BestTimeForVisit { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public Specialty DoctorSpecialty { get; set; }
        public Degree DoctorDegree { get; set; }
    }
}
