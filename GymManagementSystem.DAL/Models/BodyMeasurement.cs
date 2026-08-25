using System;

namespace GymManagementSystem.DAL.Models
{
    public class BodyMeasurement : Base
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;
        public DateTime Date { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal? BodyFat { get; set; }
        public string? Notes { get; set; }
    }
}
