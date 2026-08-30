using GymManagement.Models;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.BLL.ViewModels.Payments;
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
        private readonly IDiscountService _discountService;

        public MembershipService(IUnitOfWork unitOfWork, IDiscountService discountService)
        {
            _unitOfWork = unitOfWork;
            _discountService = discountService;
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.membershipRepository.GetAllMembershipsWithDetailsAsync(ct);
            if (memberships is null) return Enumerable.Empty<MembershipViewModel>();

            var membershipsDTOs = memberships.Select(m => new MembershipViewModel()
            {
                Id = m.Id,
                MemberId = m.MemberId,
                MemberName = m.Member.Name,
                PlanId = m.PlanId,
                PlanName = m.Plan.Name,
                Price = m.Plan.Price,
                StartDate = m.CreatedAt,
                EndDate = m.EndDate,
                Status = m.Status,
                DiscountCode = m.DiscountCode,
                DiscountAmount = m.DiscountAmount
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
                EndDate = membership.EndDate,
                Status = membership.Status,
                DiscountCode = membership.DiscountCode,
                DiscountAmount = membership.DiscountAmount
            };

            return Result<MembershipViewModel>.Ok(membershipDTO);
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepositories<Member>().GetAllAsync(ct: ct);
            if (members is null) return Enumerable.Empty<MemberSelectViewModel>();

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
            if (plans is null) return Enumerable.Empty<PlanSelectViewModel>();

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

            decimal discountAmount = 0m;
            string? discountCode = null;
            if (!string.IsNullOrWhiteSpace(model.DiscountCode))
            {
                var (valid, discounted, _) = await _discountService.ValidateAsync(model.DiscountCode, plan.Price, ct);
                if (valid)
                {
                    discountCode = model.DiscountCode.Trim();
                    discountAmount = Math.Round(plan.Price - discounted, 2, MidpointRounding.AwayFromZero);
                }
            }

            var membership = new MemberShip()
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                Status = "Active",
                DiscountCode = discountCode,
                DiscountAmount = discountAmount
            };

            _unitOfWork.membershipRepository.Add(membership);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to create membership");
        }

        public async Task<Result> CancelAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.GetRepositories<MemberShip>().GetByIdAsync(id, ct);
            if (membership is null) return Result.NotFound($"Membership with id {id} not found");

            if (membership.EndDate <= DateTime.UtcNow) return Result.Fail("Membership is already expired");

            membership.EndDate = DateTime.UtcNow;
            membership.Status = "Cancelled";
            membership.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepositories<MemberShip>().Update(membership);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to cancel membership");
        }

        public async Task<Result<CreatePaymentViewModel>> GetRecordPaymentModelAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(id, ct);
            if (membership is null) return Result<CreatePaymentViewModel>.NotFound($"Membership with id {id} not found");

            var basePrice = membership.Plan.Price;
            var payable = basePrice - membership.DiscountAmount;
            if (payable < 0) payable = 0;

            var model = new CreatePaymentViewModel
            {
                MembershipId = membership.Id,
                MembershipLabel = $"{membership.Member.Name} - {membership.Plan.Name}",
                Amount = payable,
                DiscountCode = membership.DiscountCode
            };

            return Result<CreatePaymentViewModel>.Ok(model);
        }

        public async Task<Result> CreatePaymentAsync(CreatePaymentViewModel model, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(model.MembershipId, ct);
            if (membership is null) return Result.NotFound($"Membership with id {model.MembershipId} not found");

            var payment = new Payment
            {
                MembershipId = membership.Id,
                Amount = model.Amount,
                PaymentDate = DateTime.UtcNow,
                Method = model.Method,
                Reference = model.Reference,
                Notes = model.Notes
            };

            _unitOfWork.GetRepositories<Payment>().Add(payment);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to record payment");
        }

        public async Task<IEnumerable<PaymentViewModel>> GetPaymentsByMembershipAsync(int membershipId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(membershipId, ct);
            if (membership is null) return Enumerable.Empty<PaymentViewModel>();

            var payments = await _unitOfWork.GetRepositories<Payment>()
                .GetAllAsync(tracking: false, ct: ct);

            return payments
                .Where(p => p.MembershipId == membershipId)
                .OrderByDescending(p => p.PaymentDate)
                .Select(p => new PaymentViewModel
                {
                    Id = p.Id,
                    MembershipId = p.MembershipId,
                    MemberName = membership.Member.Name,
                    PlanName = membership.Plan.Name,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Method = p.Method,
                    Reference = p.Reference,
                    Notes = p.Notes
                })
                .ToList();
        }

        public async Task<IEnumerable<PaymentViewModel>> GetAllPaymentsAsync(CancellationToken ct = default)
        {
            var payments = await _unitOfWork.GetRepositories<Payment>()
                .GetAllAsync(tracking: false, ct: ct);

            var memberships = await _unitOfWork.membershipRepository.GetAllMembershipsWithDetailsAsync(ct);
            var map = memberships.ToDictionary(m => m.Id, m => m);

            return payments
                .OrderByDescending(p => p.PaymentDate)
                .Select(p =>
                {
                    map.TryGetValue(p.MembershipId, out var ms);
                    return new PaymentViewModel
                    {
                        Id = p.Id,
                        MembershipId = p.MembershipId,
                        MemberName = ms?.Member.Name ?? "—",
                        PlanName = ms?.Plan.Name ?? "—",
                        Amount = p.Amount,
                        PaymentDate = p.PaymentDate,
                        Method = p.Method,
                        Reference = p.Reference,
                        Notes = p.Notes
                    };
                })
                .ToList();
        }

        public async Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default)
        {
            var payments = await _unitOfWork.GetRepositories<Payment>().GetAllAsync(tracking: false, ct: ct);
            return payments.Sum(p => p.Amount);
        }

        public async Task<Result> RenewAsync(int id, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(id, ct);
            if (membership is null) return Result.NotFound($"Membership with id {id} not found");

            var extension = membership.Plan.DurationDays > 0
                ? membership.Plan.DurationDays
                : 30;

            var baseDate = membership.EndDate > DateTime.UtcNow ? membership.EndDate : DateTime.UtcNow;
            membership.EndDate = baseDate.AddDays(extension);
            membership.Status = "Active";
            membership.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepositories<MemberShip>().Update(membership);

            var payable = membership.Plan.Price - membership.DiscountAmount;
            if (payable < 0) payable = 0;

            var payment = new Payment
            {
                MembershipId = membership.Id,
                Amount = payable,
                PaymentDate = DateTime.UtcNow,
                Method = "Renewal",
                Notes = $"Renewal for {extension} days"
            };
            _unitOfWork.GetRepositories<Payment>().Add(payment);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to renew membership");
        }

        public async Task<(bool valid, decimal discountedPrice, string? error)> GetDiscountedPriceAsync(string? discountCode, decimal basePrice, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(discountCode))
                return (false, basePrice, "No discount code provided");

            var (valid, discounted, _) = await _discountService.ValidateAsync(discountCode, basePrice, ct);
            if (!valid)
                return (false, basePrice, "Invalid discount code");

            return (true, discounted, null);
        }

        public async Task<Result<int>> CreateForMemberAsync(int memberId, int planId, string? discountCode, decimal discountAmount, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return Result<int>.NotFound("Member not found");

            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return Result<int>.NotFound("Plan not found");
            if (!plan.IsActive) return Result<int>.Validation("Cannot subscribe to an inactive plan");

            var activeMembership = await _unitOfWork.membershipRepository.GetActiveMembershipByMemberIdAsync(memberId, ct);
            if (activeMembership is not null) return Result<int>.Fail("Member already has an active membership");

            var membership = new MemberShip()
            {
                MemberId = memberId,
                PlanId = planId,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                Status = "Active",
                DiscountCode = discountCode,
                DiscountAmount = discountAmount
            };

            _unitOfWork.membershipRepository.Add(membership);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result<int>.Ok(membership.Id) : Result<int>.Fail("Failed to create membership");
        }

        public async Task<Result<int>> CreatePendingForMemberAsync(int memberId, int planId, string? discountCode, decimal discountAmount, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return Result<int>.NotFound("Member not found");

            var plan = await _unitOfWork.GetRepositories<Plan>().GetByIdAsync(planId, ct);
            if (plan is null) return Result<int>.NotFound("Plan not found");
            if (!plan.IsActive) return Result<int>.Validation("Cannot subscribe to an inactive plan");

            var activeMembership = await _unitOfWork.membershipRepository.GetActiveMembershipByMemberIdAsync(memberId, ct);
            if (activeMembership is not null) return Result<int>.Fail("Member already has an active membership");

            var membership = new MemberShip()
            {
                MemberId = memberId,
                PlanId = planId,
                EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                Status = "Pending",
                DiscountCode = discountCode,
                DiscountAmount = discountAmount
            };

            _unitOfWork.membershipRepository.Add(membership);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result<int>.Ok(membership.Id) : Result<int>.Fail("Failed to create pending membership");
        }

        public async Task<Result> ActivateMembershipAsync(int membershipId, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.GetRepositories<MemberShip>().GetByIdAsync(membershipId, ct);
            if (membership is null) return Result.NotFound("Membership not found");

            membership.Status = "Active";
            membership.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.GetRepositories<MemberShip>().Update(membership);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0 ? Result.Ok() : Result.Fail("Failed to activate membership");
        }

        public async Task<Result> RecordMemberPaymentAsync(int membershipId, decimal amount, string method, string? reference, string? notes, CancellationToken ct = default)
        {
            var membership = await _unitOfWork.membershipRepository.GetMembershipByIdWithDetailsAsync(membershipId, ct);
            if (membership is null) return Result.NotFound("Membership not found");

            var payment = new Payment
            {
                MembershipId = membershipId,
                Amount = amount,
                PaymentDate = DateTime.UtcNow,
                Method = method,
                Reference = reference,
                Notes = notes
            };

            _unitOfWork.GetRepositories<Payment>().Add(payment);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to record payment");
        }
    }
}
