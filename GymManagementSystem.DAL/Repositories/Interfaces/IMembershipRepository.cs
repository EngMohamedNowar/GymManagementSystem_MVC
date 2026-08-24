using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IMembershipRepository : IGenericRepositories<MemberShip>
    {
        Task<IEnumerable<MemberShip>> GetAllMembershipsWithDetailsAsync(CancellationToken ct = default);
        Task<MemberShip?> GetMembershipByIdWithDetailsAsync(int id, CancellationToken ct = default);
        Task<MemberShip?> GetActiveMembershipByMemberIdAsync(int memberId, CancellationToken ct = default);
    }
}
