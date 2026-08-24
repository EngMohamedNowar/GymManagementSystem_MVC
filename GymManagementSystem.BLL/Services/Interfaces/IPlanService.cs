using GymManagementSystem.BLL.ViewModes.Plans;
using System.Linq.Expressions;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllAsync(CancellationToken ct = default);

        Task<PlanViewModel?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<(bool Success, string Message)> TogglePlanStatus(int id,CancellationToken ct);

        //Task<bool> CreateAsync(TCreateDto model, CancellationToken ct = default);

        Task<PlanViewModel?> GetForUpdateAsync(int id, CancellationToken ct = default);

        Task<bool> UpdateAsync(PlanViewModel model, CancellationToken ct = default);

        //Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}