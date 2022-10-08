using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models.ViewModels
{
    public class TaskVM
    {
        public int Id { get; set; }
        public string comment { get; set; }
        [Display(Name ="Time of visit")]
        public string TimeOfVisit { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Doctor name")]
        public string DoctorName { get; set; }
        [Display(Name = "Doctor specialty")]
        public string DoctorSpecialty { get; set; }
        [Display(Name = "Doctor degree")]
        public string DoctorDegree { get; set; }
        public string Type { get; set; }
        [Display(Name = "Hospital name")]
        public string? HospitalName { get; set; }
        [Display(Name = "Hospital address")]
        public string? HospitalAddress { get; set; }
        [Display(Name = "Hospital Phone 1")]
        public string? HospitalPhone1 { get; set; }
        [Display(Name = "Clinic Address")]
        public string? ClinicAddress { get; set; }
        [Display(Name = "Clinic Phone 1")]
        public string? ClinicPhone1 { get; set; }
    }
}
