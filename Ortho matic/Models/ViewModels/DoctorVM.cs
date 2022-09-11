using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Ortho_matic.Models.ViewModels
{
    public class DoctorVM
    {
        public Doctor Doctor { get; set; }
        public IEnumerable<SelectListItem> DoctorDegreeSelectList { get; set; }
        public IEnumerable<SelectListItem> DoctorSpecialtySelectList { get; set; }
    }
}
