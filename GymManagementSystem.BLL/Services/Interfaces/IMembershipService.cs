using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.BLL.ViewModels.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result<MembershipViewModel>> GetDetailsAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectViewModel>> GetAllPlansForDropDownAsync(CancellationToken ct = default);
        Task<Result> CreateAsync(CreateMembershipViewModel model, CancellationToken ct = default);
        Task<Result> CancelAsync(int id, CancellationToken ct = default);
        Task<Result<CreatePaymentViewModel>> GetRecordPaymentModelAsync(int id, CancellationToken ct = default);
        Task<Result> CreatePaymentAsync(CreatePaymentViewModel model, CancellationToken ct = default);
        Task<IEnumerable<PaymentViewModel>> GetPaymentsByMembershipAsync(int membershipId, CancellationToken ct = default);
        Task<IEnumerable<PaymentViewModel>> GetAllPaymentsAsync(CancellationToken ct = default);
        Task<decimal> GetTotalRevenueAsync(CancellationToken ct = default);
        Task<Result> RenewAsync(int id, CancellationToken ct = default);
    }
}
