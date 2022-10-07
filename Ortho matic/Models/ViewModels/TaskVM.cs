using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Ortho_matic.Models.ViewModels
{
    public class TaskVM
    {
        public int Id { get; set; }
        public string comment { get; set; }
        public string TimeOfVisit { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string UserName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialty { get; set; }
        public string DoctorDegree { get; set; }
        public string Type { get; set; }
        public string? HospitalName { get; set; }
        public string? HospitalAddress { get; set; }
        public string? HospitalPhone1 { get; set; }
        public string? ClinicAddress { get; set; }
        public string? ClinicPhone1 { get; set; }
    }
}
