using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Discounts;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool valid, decimal discountedAmount, string? error)> ValidateAsync(string code, decimal amount, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code))
                return (false, amount, "Discount code is empty");

            var all = await _unitOfWork.GetRepositories<Discount>().GetAllAsync(tracking: false, ct: ct);
            var discount = all.FirstOrDefault(d => string.Equals(d.Code, code.Trim(), StringComparison.OrdinalIgnoreCase));

            if (discount is null)
                return (false, amount, "Invalid discount code");

            if (!discount.IsActive)
                return (false, amount, "Discount code is not active");

            var now = DateTime.UtcNow;
            if (discount.StartDate is not null && now < discount.StartDate)
                return (false, amount, "Discount code is not active yet");

            if (discount.EndDate is not null && now > discount.EndDate)
                return (false, amount, "Discount code has expired");

            var discounted = discount.Type == "Fixed"
                ? amount - discount.Value
                : amount * (1 - discount.Value / 100m);

            if (discounted < 0) discounted = 0;

            discounted = Math.Round(discounted, 2, MidpointRounding.AwayFromZero);

            return (true, discounted, null);
        }

        public async Task<IEnumerable<DiscountViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var all = await _unitOfWork.GetRepositories<Discount>().GetAllAsync(tracking: false, ct: ct);
            return all
                .OrderBy(d => d.Code)
                .Select(d => new DiscountViewModel
                {
                    Id = d.Id,
                    Code = d.Code,
                    Type = d.Type,
                    Value = d.Value,
                    Description = d.Description,
                    IsActive = d.IsActive,
                    StartDate = d.StartDate,
                    EndDate = d.EndDate,
                    CreatedAt = d.CreatedAt
                })
                .ToList();
        }

        public async Task<Result> CreateAsync(CreateDiscountViewModel model, CancellationToken ct = default)
        {
            if (model is null) return Result.Validation("Model is required");

            var all = await _unitOfWork.GetRepositories<Discount>().GetAllAsync(tracking: false, ct: ct);
            if (all.Any(d => string.Equals(d.Code, model.Code.Trim(), StringComparison.OrdinalIgnoreCase)))
                return Result.Validation("A discount with this code already exists");

            var discount = new Discount
            {
                Code = model.Code.Trim(),
                Type = model.Type == "Fixed" ? "Fixed" : "Percent",
                Value = model.Value,
                Description = model.Description,
                IsActive = model.IsActive,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            _unitOfWork.GetRepositories<Discount>().Add(discount);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0 ? Result.Ok() : Result.Fail("Failed to create discount");
        }

        public async Task<Result> ToggleActiveAsync(int id, CancellationToken ct = default)
        {
            var discount = await _unitOfWork.GetRepositories<Discount>().GetByIdAsync(id, ct);
            if (discount is null) return Result.NotFound($"Discount with id {id} not found");

            discount.IsActive = !discount.IsActive;
            _unitOfWork.GetRepositories<Discount>().Update(discount);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0 ? Result.Ok() : Result.Fail("Failed to update discount");
        }
    }
}
