using System;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class MemberBookingViewModel
    {
        public int BookingId { get; set; }
        public int SessionId { get; set; }
        public string SessionTitle { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsAttended { get; set; }
        public bool CanCancel { get; set; }
    }
}
