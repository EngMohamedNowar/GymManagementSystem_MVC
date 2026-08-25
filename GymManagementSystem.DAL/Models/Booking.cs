using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Booking : Base
    {
        public Member Member { get; set; } = null!;
        public int MemberId { get; set; }
        public Session Session { get; set; } = null!;
        public int SessionId { get; set; }
        public bool IsAttended { get; set; }
    }
}
