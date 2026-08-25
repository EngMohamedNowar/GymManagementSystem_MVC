using GymManagementSystem.BLL.ViewModels.Home;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IHomeService
    {
        Task<AnalyticsViewModel> GetDashboardStatsAsync();
    }
}
