using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.Payments
{
    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int MembershipId { get; set; }
        public string MemberName { get; set; } = default!;
        public string PlanName { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = default!;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
