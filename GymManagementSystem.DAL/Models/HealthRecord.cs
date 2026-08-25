using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class HealthRecord : Base
    {
        public decimal Height { get; set; }
        public decimal weight { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public string? Note { get; set; }
        public Member Member { get; set; } = null!;
    }
}
