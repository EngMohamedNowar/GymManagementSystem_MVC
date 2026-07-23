using GymManagementSystem.BLL.ViewModes.Members;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
        Task <MemberViewModel?> GetMemberDetalisAsync(int memberId,CancellationToken ct = default);
        Task<bool> CreateMemberAsync(CreateMemberDTOs model, CancellationToken ct = default);
        Task<UpdateMemberDTOs> MemberToUpdateAsync(int meberId, CancellationToken ct = default);
        Task<bool> UpdateMemberAsync(int memberId, UpdateMemberDTOs model, CancellationToken ct = default);
        Task<bool> DeleteMemberAsync(int meberId, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetMemberHelthRecordAsync(int memberId, CancellationToken ct);
    }
}
