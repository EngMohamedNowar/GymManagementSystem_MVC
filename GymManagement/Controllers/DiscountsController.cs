using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Discounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DiscountsController : Controller
    {
        private readonly IDiscountService _discountService;
        private readonly IAuditService _auditService;

        public DiscountsController(IDiscountService discountService, IAuditService auditService)
        {
            _discountService = discountService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var discounts = await _discountService.GetAllAsync(ct);
            return View(discounts);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateDiscountViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CreateDiscountViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _discountService.CreateAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Discount created successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Create Discount", "Discount", null, model.Code, ct);
            }
            else
            {
                TempData["FailedMessage"] = result.error;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id, CancellationToken ct = default)
        {
            var result = await _discountService.ToggleActiveAsync(id, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Discount updated";
                await _auditService.LogAsync(User.Identity?.Name, "Toggle Discount", "Discount", id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = result.error;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
