using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Common;
using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.BLL.ViewModels.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class MembershipsController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IAuditService _auditService;

        public MembershipsController(IMembershipService membershipService, IAuditService auditService)
        {
            _membershipService = membershipService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? status, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            var all = (await _membershipService.GetAllAsync(ct))?.ToList() ?? new List<MembershipViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                all = all.Where(m =>
                    (m.MemberName?.ToLower().Contains(s) ?? false) ||
                    (m.PlanName?.ToLower().Contains(s) ?? false)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                all = all.Where(m => string.Equals(m.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var paged = new PaginationViewModel<MembershipViewModel>
            {
                Items = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = all.Count
            };

            ViewBag.Search = search;
            ViewBag.Status = status;
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _membershipService.GetDetailsAsync(id, ct);
            if (result.success)
            {
                ViewBag.Payments = await _membershipService.GetPaymentsByMembershipAsync(id, ct);
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
                    await _auditService.LogAsync(User.Identity?.Name, "Create Membership", "Membership", null, $"Member {model.MemberId} / Plan {model.PlanId}", ct);
                }
                else
                {
                    TempData["FailedMessage"] = result.error;
                }
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

            if (result.success)
                await _auditService.LogAsync(User.Identity?.Name, "Cancel Membership", "Membership", id.ToString(), null, ct);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RecordPayment(int membershipId, CancellationToken ct)
        {
            var result = await _membershipService.GetRecordPaymentModelAsync(membershipId, ct);
            if (!result.success)
            {
                TempData["FailedMessage"] = result.error;
                return RedirectToAction(nameof(Details), new { id = membershipId });
            }
            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> RecordPayment(CreatePaymentViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _membershipService.CreatePaymentAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Payment recorded successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Record Payment", "Payment", null, $"Membership {model.MembershipId} / {model.Amount}", ct);
            }
            else
            {
                TempData["FailedMessage"] = result.error;
            }
            return RedirectToAction(nameof(Details), new { id = model.MembershipId });
        }

        [HttpPost]
        public async Task<IActionResult> Renew(int id, CancellationToken ct)
        {
            var result = await _membershipService.RenewAsync(id, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership renewed successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Renew Membership", "Membership", id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = result.error;
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
