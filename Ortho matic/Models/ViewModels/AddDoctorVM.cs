using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Ortho_matic.Models.ViewModels
{
    public class AddDoctorVM
    {
        public Doctor Doctor { get; set; }
        public IEnumerable<SelectListItem> DoctorDegreeSelectList { get; set; }
        public IEnumerable<SelectListItem> DoctorSpecialtySelectList { get; set; }
        public IEnumerable<SelectListItem> ClinicSelectList { get; set; }
        public IEnumerable<SelectListItem> HospitsalSelectList { get; set; }
        public IEnumerable<SelectListItem> DaySelectList { get; set; }

    }
}
