using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.BodyMeasurements;
using GymManagementSystem.BLL.ViewModels.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Member")]
    public class MeasurementsController : Controller
    {
        private readonly IBodyMeasurementService _bodyMeasurementService;
        private readonly IMemberService _memberService;
        private readonly IAuditService _auditService;

        public MeasurementsController(IBodyMeasurementService bodyMeasurementService,
            IMemberService memberService,
            IAuditService auditService)
        {
            _bodyMeasurementService = bodyMeasurementService;
            _memberService = memberService;
            _auditService = auditService;
        }

        private async Task<int?> ResolveMemberIdAsync(int? memberId, CancellationToken ct)
        {
            if (memberId.HasValue) return memberId;

            if (User.IsInRole("Member"))
            {
                var email = User.Identity?.Name;
                var me = await _memberService.GetMemberByEmailAsync(email, ct);
                return me?.Id;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? memberId, CancellationToken ct = default)
        {
            var resolvedId = await ResolveMemberIdAsync(memberId, ct);
            if (!resolvedId.HasValue)
            {
                TempData["FailedMessage"] = "Please select a member";
                ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
                return View(Enumerable.Empty<BodyMeasurementViewModel>());
            }

            var measurements = await _bodyMeasurementService.GetByMemberAsync(resolvedId.Value, ct);
            ViewBag.MemberId = resolvedId.Value;

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
            }

            return View(measurements);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? memberId, CancellationToken ct = default)
        {
            var isMemberOnly = User.IsInRole("Member") && !(User.IsInRole("SuperAdmin") || User.IsInRole("Admin"));

            int? resolvedId = null;
            if (!isMemberOnly)
            {
                resolvedId = await ResolveMemberIdAsync(memberId, ct);
                ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
                ViewBag.CanChoose = true;
            }
            else
            {
                resolvedId = await ResolveMemberIdAsync(null, ct);
                ViewBag.CanChoose = false;
            }

            if (!resolvedId.HasValue)
            {
                TempData["FailedMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }

            var model = new CreateBodyMeasurementViewModel { MemberId = resolvedId.Value };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBodyMeasurementViewModel model, CancellationToken ct = default)
        {
            var isMemberOnly = User.IsInRole("Member") && !(User.IsInRole("SuperAdmin") || User.IsInRole("Admin"));
            if (isMemberOnly)
            {
                var me = await ResolveMemberIdAsync(null, ct);
                if (me.HasValue) model.MemberId = me.Value;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
                ViewBag.CanChoose = !isMemberOnly;
                return View(model);
            }

            var result = await _bodyMeasurementService.AddAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Measurement added";
                await _auditService.LogAsync(User.Identity?.Name, "Add Measurement", "BodyMeasurement", null, $"Member {model.MemberId}", ct);
                return RedirectToAction(nameof(Index), new { memberId = model.MemberId });
            }

            TempData["FailedMessage"] = result.error;
            ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
            ViewBag.CanChoose = !isMemberOnly;
            return View(model);
        }
    }
}
