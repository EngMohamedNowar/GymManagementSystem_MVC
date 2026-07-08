using GymManagement.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IPlanRepositories
    {
        Task<int> AddAsync(Plan plan, CancellationToken ct = default);
        Task<Plan?> GetByIdAsync(int id,CancellationToken ct =default);
        Task<int> UpdateAsync(Plan plan, CancellationToken ct = default);
        Task<int> DeleteAsync(Plan plan, CancellationToken ct = default);
        Task<IEnumerable<Plan>> GetAllPlansAsync(bool tracking = false, CancellationToken ct = default);

    }
}
