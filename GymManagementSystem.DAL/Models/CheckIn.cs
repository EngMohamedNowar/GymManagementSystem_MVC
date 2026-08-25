using System;

namespace GymManagementSystem.DAL.Models
{
    public class CheckIn : Base
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;
        public DateTime CheckInTime { get; set; }
        public string? Note { get; set; }
    }
}
