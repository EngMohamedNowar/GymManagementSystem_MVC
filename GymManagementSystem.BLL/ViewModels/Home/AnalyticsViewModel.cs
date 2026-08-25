using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Home
{
    public class AnalyticsViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int TotalTrainers { get; set; }
        public int UpcomingSessions { get; set; }
        public int OngoingSessions { get; set; }
        public int CompletedSessions { get; set; }

        public decimal TotalRevenue { get; set; }
        public int TotalMemberships { get; set; }
        public int ExpiredMemberships { get; set; }
        public int PaymentsCount { get; set; }
        public List<PlanDistributionItem> PlanDistribution { get; set; } = new();
    }
}
