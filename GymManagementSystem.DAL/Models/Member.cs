using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }
        public HealthRecord Health { get; set; }
        public int HealthId { get; set; }
        public ICollection<MemberShip> Plans { get; set; }
        public ICollection<Booking> Sessions { get; set; }
    }
}
