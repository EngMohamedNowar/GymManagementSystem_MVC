using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Bookings
{
    public class BookingViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = default!;
        public string MemberPhone { get; set; } = default!;

        public int SessionId { get; set; }
        public string SessionCategoryName { get; set; } = default!;
        public string TrainerName { get; set; } = default!;
        public DateTime SessionStartDate { get; set; }
        public DateTime SessionEndDate { get; set; }

        public bool IsAttended { get; set; }
        public DateTime BookedAt { get; set; }

        // Computed properties
        public string DateDisplay => $"{SessionStartDate:MMM dd, yyyy}";
        public string TimeRangeDisplay => $"{SessionStartDate:hh:mm tt} - {SessionEndDate:hh:mm tt}";
        public string Status
        {
            get
            {
                if (SessionStartDate > DateTime.Now)
                    return "Upcoming";
                else if (SessionStartDate <= DateTime.Now && SessionEndDate >= DateTime.Now)
                    return "Ongoing";
                else
                    return "Completed";
            }
        }
        public bool CanCancel => SessionStartDate > DateTime.Now;
    }
}
