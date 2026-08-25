using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Models
{
    public class Discount : Base
    {
        public string Code { get; set; } = default!;
        public string Type { get; set; } = "Percent"; // "Percent" | "Fixed"
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
