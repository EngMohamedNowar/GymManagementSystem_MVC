using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Home;
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
            var now = DateTime.UtcNow;

            var memberships = await _unitOfWork.membershipRepository.GetAllMembershipsWithDetailsAsync();

            var revenue = (await _unitOfWork.GetRepositories<Payment>().GetAllAsync(tracking: false))
                .Sum(p => p.Amount);

            var planDistribution = memberships
                .GroupBy(m => m.Plan.Name)
                .Select(g => new PlanDistributionItem { PlanName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

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
                    .CountAsync(s => s.EndDate < now),

                TotalRevenue = revenue,
                TotalMemberships = memberships.Count(),
                ExpiredMemberships = memberships.Count(m => m.Status == "Expired"),
                PaymentsCount = await _unitOfWork.GetRepositories<Payment>().CountAsync(),
                PlanDistribution = planDistribution
            };
        }
    }
}
