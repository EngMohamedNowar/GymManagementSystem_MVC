using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Home;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class HomeServices : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;

            return new AnalyticsViewModel()
            {
                TotalMembers = await _unitOfWork.GetRepositories<Member>().CountAsync(),

                ActiveMembers = await _unitOfWork.GetRepositories<MemberShip>()
                    .CountAsync(m => m.EndDate > now),

                TotalTrainers = await _unitOfWork.GetRepositories<Trainer>().CountAsync(),

                UpcomingSessions = await _unitOfWork.sessionRepository.CountAsync(s => s.StartDate > now),

                OngoingSessions = await _unitOfWork.sessionRepository
                    .CountAsync(s => s.StartDate <= now && s.EndDate >= now),

                CompletedSessions = await _unitOfWork.sessionRepository
                    .CountAsync(s => s.EndDate < now)
            };
        }
    }
}
