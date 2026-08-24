using GymManagement.DbContexts;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Plans;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class PlansController :Controller
    {

        private readonly IPlanService _planService;
        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        // Index
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct= default)
        {
            var plans = await _planService.GetAllAsync(ct:ct);

            return View(plans); // name inedex .cshtml
        }
        // Details
        [HttpGet]
        public async Task<IActionResult> Details(int id,CancellationToken ct = default)
        {
            //var plan = await _Context.Plans.FirstOrDefaultAsync(p => p.Id == id);
            var plan = await _planService.GetByIdAsync(id,ct:ct);

            if (plan is null) return RedirectToAction(nameof(Index));
            return View(plan); // name details.cshtml
        }
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, CancellationToken ct)
        {
            var (success, message) = await _planService.TogglePlanStatus(id, ct);

            TempData["FlashMessage"] = message;
            TempData["FlashSuccess"] = success;

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

            return RedirectToAction(nameof(Index));
        }

    }
}
