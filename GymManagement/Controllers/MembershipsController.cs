using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var memberships = await _membershipService.GetAllAsync(ct);
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _membershipService.GetDetailsAsync(id, ct);
            if (result.success)
            {
                return View(result.value);
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Members = new SelectList(await _membershipService.GetAllMembersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Plans = new SelectList(await _membershipService.GetAllPlansForDropDownAsync(ct), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                var result = await _membershipService.CreateAsync(model, ct);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Membership Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["FailedMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Members = new SelectList(await _membershipService.GetAllMembersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Plans = new SelectList(await _membershipService.GetAllPlansForDropDownAsync(ct), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var result = await _membershipService.CancelAsync(id, ct);

            TempData["FlashMessage"] = result.error ?? "Membership Cancelled Successfully";
            TempData["FlashSuccess"] = result.success;

            return RedirectToAction(nameof(Index));
        }
    }
}
