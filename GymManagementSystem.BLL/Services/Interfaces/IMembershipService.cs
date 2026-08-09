using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModes.Memberships;
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
    }
}
