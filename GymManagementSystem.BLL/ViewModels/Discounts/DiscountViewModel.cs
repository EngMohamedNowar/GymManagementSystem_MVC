using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.Discounts
{
    public class DiscountViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string Type { get; set; } = default!;
        public decimal Value { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
