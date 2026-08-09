using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModes.Memberships
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
        public bool IsActive => EndDate > DateTime.UtcNow;
        public string Status => EndDate > DateTime.UtcNow ? "Active" : "Expired";
    }
}
