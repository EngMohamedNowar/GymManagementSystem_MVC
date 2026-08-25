using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Discounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<(bool valid, decimal discountedAmount, string? error)> ValidateAsync(string code, decimal amount, CancellationToken ct = default);
        Task<IEnumerable<DiscountViewModel>> GetAllAsync(CancellationToken ct = default);
        Task<Result> CreateAsync(CreateDiscountViewModel model, CancellationToken ct = default);
        Task<Result> ToggleActiveAsync(int id, CancellationToken ct = default);
    }
}
