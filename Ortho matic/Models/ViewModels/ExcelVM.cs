using System;
using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models.ViewModels
{
    public class ExcelVM
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "From time")]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "To time")]
        public DateTime EndTime { get; set; }
    }
}
