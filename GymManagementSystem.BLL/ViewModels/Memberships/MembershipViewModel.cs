using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Memberships
{
    public class MembershipViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = default!;
        public int PlanId { get; set; }
        public string PlanName { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive => Status == "Active";
        public string Status { get; set; } = default!;
        public string? DiscountCode { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
