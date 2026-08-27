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

        private async Task<(int? MemberId, string? Error)> ResolveMemberIdAsync(int? memberId, CancellationToken ct)
        {
            var isStaff = User.IsInRole("SuperAdmin") || User.IsInRole("Admin");

            if (User.IsInRole("Member") && !isStaff)
            {
                var email = User.Identity?.Name ?? string.Empty;
                if (string.IsNullOrWhiteSpace(email)) return (null, "Member profile not found");
                var me = await _memberService.GetMemberByEmailAsync(email, ct);
                return (me?.Id, me is null ? "Member profile not found" : null);
            }

            if (isStaff && memberId.HasValue)
                return (memberId.Value, null);

            return (null, "Please select a member");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? memberId, CancellationToken ct = default)
        {
            var (resolvedId, error) = await ResolveMemberIdAsync(memberId, ct);
            if (!resolvedId.HasValue)
            {
                TempData["FailedMessage"] = error ?? "Please select a member";
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
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
                resolvedId = memberId;
                ViewBag.Members = await _memberService.GetAllMembersAsync(ct);
                ViewBag.CanChoose = true;
            }
            else
            {
                var (meId, _) = await ResolveMemberIdAsync(null, ct);
                resolvedId = meId;
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
                var (meId, _) = await ResolveMemberIdAsync(null, ct);
                if (meId.HasValue) model.MemberId = meId.Value;
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
