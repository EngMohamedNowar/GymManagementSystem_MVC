using GymManagement.Models;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Plans;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepositories<Plan> _planRepository;
        private readonly IGenericRepositories<MemberShip> _memberShipRepository;

        public PlanService(IGenericRepositories<Plan> planRepository
            ,IGenericRepositories<MemberShip> memberShipRepository)
        {
            _planRepository = planRepository;
            _memberShipRepository = memberShipRepository;
        }


        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var plans = await _planRepository.GetAllAsync();

            if (plans is null) return null;

            var plansDto = plans.Select(plan => new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            }).ToList();
            return plansDto;
        }


        public async Task<PlanViewModel?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);
            if (plan is null) return null;
            var planDtos = new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };
            return planDtos;
        }

        public async Task<(bool Success, string Message)> TogglePlanStatus(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan is null)
                return (false, "Plan not found");

            // لو الخطة شغالة دلوقتي وعايز تعملها Deactivate، تأكد مفيش اشتراكات فعالة عليها
            if (plan.IsActive)
            {
                var hasActiveMemberships = await _memberShipRepository.AnyAsync(m => m.PlanId == id && m.EndDate > DateTime.UtcNow,ct);

                if (hasActiveMemberships)
                    return (false, "Cannot deactivate a plan with active memberships");
            }

            plan.IsActive = !plan.IsActive;
            var count = await _planRepository.UpdateAsync(plan);

            if (count <= 0)
                return (false, "Failed to update plan status");

            return (true, "Plan Status Changed");
        }
        public async Task<PlanViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(id);

            if (plan is null)
                return null;

            var planViewModel = new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                Duration = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };

            return planViewModel;
        }



        public async Task<bool> UpdateAsync(PlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _planRepository.GetByIdAsync(model.Id);

            if (plan is null)
                return false;
            plan.Id = model.Id;
            plan.Name = model.Name;
            plan.Price = model.Price;
            plan.DurationDays = model.Duration;
            plan.Description = model.Description;
            plan.IsActive = model.IsActive;
            var count = await _planRepository.UpdateAsync(plan);

            return count > 0;
        }
    }
}
