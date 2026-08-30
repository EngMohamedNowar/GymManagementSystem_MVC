using GymManagement.DbContexts;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Common;
using GymManagementSystem.BLL.ViewModels.Plans;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class PlansController : Controller
    {
        private readonly IPlanService _planService;
        private readonly IAuditService _auditService;

        public PlansController(IPlanService planService, IAuditService auditService)
        {
            _planService = planService;
            _auditService = auditService;
        }
        // Index
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            var all = (await _planService.GetAllAsync(ct: ct))?.ToList() ?? new List<PlanViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                all = all.Where(p =>
                    (p.Name?.ToLower().Contains(s) ?? false) ||
                    (p.Description?.ToLower().Contains(s) ?? false)).ToList();
            }

            var paged = new PaginationViewModel<PlanViewModel>
            {
                Items = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = all.Count
            };

            ViewBag.Search = search;
            return View(paged);
        }
        // Details
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var plan = await _planService.GetByIdAsync(id, ct: ct);

            if (plan is null) return RedirectToAction(nameof(Index));
            return View(plan);
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken ct)
        {
            var (success, message) = await _planService.TogglePlanStatus(id, ct);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                await _auditService.LogAsync(User.Identity?.Name, "Toggle Plan Status", "Plan", id.ToString(), message, ct);
            }
            else
            {
                TempData["FailedMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var planUpdated = await _planService.GetForUpdateAsync(id, ct);
            if (planUpdated is null)
            {
                TempData["FailedMessage"] = "plan is Not Found";
                return RedirectToAction("Index");

            }
            return View(planUpdated);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PlanViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var success = await _planService.UpdateAsync(model);

            if (!success)
                return NotFound();

            TempData["SuccessMessage"] = "Plan updated successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PlanViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlanViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, message) = await _planService.CreateAsync(model, ct);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                await _auditService.LogAsync(User.Identity?.Name, "Create Plan", "Plan", null, message, ct);
                return RedirectToAction(nameof(Index));
            }

            TempData["FailedMessage"] = message;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var success = await _planService.DeleteAsync(id, ct);
            if (success)
            {
                TempData["SuccessMessage"] = "Plan deleted successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Delete Plan", "Plan", id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed to delete plan or plan not found";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
