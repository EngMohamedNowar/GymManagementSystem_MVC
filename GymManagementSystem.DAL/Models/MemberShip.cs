using GymManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class MemberShip : Base
    {
        public Member Member { get; set; }
        public int MemberId { get; set; }
        public Plan Plan { get; set; }
        public int PlanId { get; set; }
        // start date ==> CreatedAt from Base
        //EndDate
        public DateTime EndDate { get; set; }
        public bool IsActive => Status == "Active";
        public string Status { get; set; } = "Active";
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public string? DiscountCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0m;

    }
}
