using GymManagement.Models;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Memberships;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.membershipRepository.GetAllMembershipsWithDetailsAsync(ct);
            if (memberships is null) return null;

            var membershipsDTOs = memberships.Select(m => new MembershipViewModel()
            {
                Id = m.Id,
                MemberId = m.MemberId,
                MemberName = m.Member.Name,
                PlanId = m.PlanId,
                PlanName = m.Plan.Name,
                Price = m.Plan.Price,
                StartDate = m.CreatedAt,
                EndDate = m.EndDate
            }).ToList();

            return membershipsDTOs;
        }

        public async Task<Result<MembershipViewModel>> GetDetailsAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(id, ct);
            if (membership is null) return Result<MembershipViewModel>.NotFound($"Membership with id {id} not found");

            var membershipDTO = new MembershipViewModel()
            {
                Id = membership.Id,
                MemberId = membership.MemberId,
                MemberName = membership.Member.Name,
                PlanId = membership.PlanId,
                PlanName = membership.Plan.Name,
                Price = membership.Plan.Price,
                StartDate = membership.CreatedAt,
                EndDate = membership.EndDate
            };

            return Result<MembershipViewModel>.Ok(membershipDTO);
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepositories<Member>().GetAllAsync(ct: ct);
            if (members is null) return null;

            var membersDTOs = members.Select(m => new MemberSelectViewModel()
            {
                Id = m.Id,
                Name = m.Name
            });

            return membersDTOs;
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetAllPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepositories<Plan>().GetAllAsync(ct: ct);
            if (plans is null) return null;

            var plansDTOs = plans.Where(p => p.IsActive).Select(p => new PlanSelectViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationDays = p.DurationDays
            });

            return plansDTOs;
        }

        public async Task<Result> CreateAsync(CreateMembershipViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(model.MemberId, ct);
            if (member is null) return Result.NotFound($"Member with id {model.MemberId} not found");

            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(model.PlanId, ct);
            if (plan is null) return Result.NotFound($"Plan with id {model.PlanId} not found");

            if (!plan.IsActive) return Result.Validation("Cannot subscribe to an inactive plan");

            var activeMembership = await _unitOfWork.membershipRepository.GetActiveMembershipByMemberIdAsync(model.MemberId, ct);
            if (activeMembership is not null) return Result.Fail("This member already has an active membership");

            var membership = new MemberShip()
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays)
            };

            _unitOfWork.membershipRepository.Add(membership);
            var count = await _unitOfWork.SaveChanegesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to create membership");
        }

        public async Task<Result> CancelAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.GetRepositories<MemberShip>().GetByIdAsync(id, ct);
            if (membership is null) return Result.NotFound($"Membership with id {id} not found");

            if (membership.EndDate <= DateTime.UtcNow) return Result.Fail("Membership is already expired");

            membership.EndDate = DateTime.UtcNow;
            membership.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepositories<MemberShip>().Update(membership);
            var count = await _unitOfWork.SaveChanegesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to cancel membership");
        }
    }
}
