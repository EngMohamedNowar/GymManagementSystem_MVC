using GymManagement.Models;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Plans;
using GymManagementSystem.DAL;
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
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepositories<Plan>().GetAllAsync();

            if (plans is null) return null;

            var plansDto = plans.Select(plan => new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            }).ToList();
            return plansDto;
        }


        public async Task<PlanViewModel?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return null;
            var planDtos = new PlanViewModel()
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };
            return planDtos;
        }

        public async Task<(bool Success, string Message)> TogglePlanStatus(int id, CancellationToken ct)
        {
            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(id);
            if (plan is null)
                return (false, "Plan not found");

            // لو الخطة شغالة دلوقتي وعايز تعملها Deactivate، تأكد مفيش اشتراكات فعالة عليها
            if (plan.IsActive)
            {
                var hasActiveMemberships = await _unitOfWork.GetRepositories<MemberShip>().AnyAsync(m => m.PlanId == id && m.EndDate > DateTime.UtcNow,ct);

                if (hasActiveMemberships)
                    return (false, "Cannot deactivate a plan with active memberships");
            }

            plan.IsActive = !plan.IsActive;

            _unitOfWork.GetRepositories<Plan>().Update(plan);
            var count = await _unitOfWork.SaveChanegesAsync(ct);

            if (count <= 0)
                return (false, "Failed to update plan status");

            return (true, "Plan Status Changed");
        }
        public async Task<PlanViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(id);

            if (plan is null)
                return null;

            var planViewModel = new PlanViewModel
            {
                Id = plan.Id,
                Name = plan.Name,
                Price = plan.Price,
                DurationDays = plan.DurationDays,
                Description = plan.Description,
                IsActive = plan.IsActive
            };

            return planViewModel;
        }



        public async Task<bool> UpdateAsync(PlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(model.Id);

            if (plan is null)
                return false;
            plan.Id = model.Id;
            plan.Name = model.Name;
            plan.Price = model.Price;
            plan.DurationDays = model.DurationDays;
            plan.Description = model.Description;
            plan.IsActive = model.IsActive;
            _unitOfWork.GetRepositories<Plan>().Update(plan);
            var count = await _unitOfWork.SaveChanegesAsync(ct);

            return count > 0;
        }
    }
}
