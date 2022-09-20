using System;
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

        public override bool Equals(object obj)
        {
            Time time = obj as Time;

            if (time == null) return false;

            if (this.GetType() != obj.GetType()) return false;

            if (object.ReferenceEquals(this, time)) return true;

            return (StartDay == time.StartDay) && (EndDay == time.EndDay) && (StartTime == time.StartTime) && (EndTime == time.EndTime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime, StartDay, EndDay);
        }
    }
}
