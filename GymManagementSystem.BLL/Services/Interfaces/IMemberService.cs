using GymManagementSystem.BLL.ViewModels.Members;
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
        Task <MemberViewModel?> GetMemberDetailsAsync(int memberId,CancellationToken ct = default);
        Task<MemberViewModel?> GetMemberByEmailAsync(string email, CancellationToken ct = default);
        Task<int> CreateMemberAsync(CreateMemberDTOs model, CancellationToken ct = default);
        Task<UpdateMemberDTOs> MemberToUpdateAsync(int memberId, CancellationToken ct = default);
        Task<bool> UpdateMemberAsync(int memberId, UpdateMemberDTOs model, CancellationToken ct = default);
        Task<MemberProfileEditViewModel?> GetMemberProfileAsync(int memberId, CancellationToken ct = default);
        Task<bool> UpdateMemberProfileAsync(int memberId, MemberProfileEditViewModel model, CancellationToken ct = default);
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default);
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct);
    }
}
