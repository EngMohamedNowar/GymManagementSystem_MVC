using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Models
{
    public class Payment : Base
    {
        public int MembershipId { get; set; }
        public MemberShip Membership { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "Cash";
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}
