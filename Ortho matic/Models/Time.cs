using System.ComponentModel.DataAnnotations;

namespace Ortho_matic.Models
{
    public class Time
    {
        public int Id { get; set; }
        [Required]
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        [Required]
        public DayOfWeekInArabic StartDay { get; set; }
        public DayOfWeekInArabic EndDay { get; set; }
    }
}
