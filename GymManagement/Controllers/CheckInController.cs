using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.CheckIns;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CheckInController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;

        public CheckInController(IUnitOfWork unitOfWork, IAuditService auditService)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Scan(int? memberId, CancellationToken ct = default)
        {
            if (memberId.HasValue)
            {
                var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId.Value, ct);
                if (member is not null)
                {
                    ViewBag.ScannedMember = member;
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Scan(int memberId, string? note, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "Member not found";
                return View();
            }

            _unitOfWork.GetRepositories<CheckIn>().Add(new CheckIn
            {
                MemberId = memberId,
                CheckInTime = DateTime.UtcNow,
                Note = note
            });

            var count = await _unitOfWork.SaveChangesAsync(ct);
            if (count > 0)
            {
                TempData["SuccessMessage"] = $"{member.Name} checked in successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Check In", "CheckIn", memberId.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed to check in";
            }

            ViewBag.ScannedMember = member;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Today(CancellationToken ct = default)
        {
            var today = DateTime.UtcNow.Date;
            var checkIns = await _unitOfWork.GetRepositories<CheckIn>().GetAllAsync(tracking: false, ct: ct);
            var todaysCheckIns = checkIns
                .Where(c => c.CheckInTime >= today)
                .OrderByDescending(c => c.CheckInTime)
                .ToList();

            var result = new System.Collections.Generic.List<CheckInViewModel>();
            foreach (var c in todaysCheckIns)
            {
                var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(c.MemberId, ct);
                result.Add(new CheckInViewModel
                {
                    Id = c.Id,
                    MemberName = member?.Name ?? "—",
                    MemberId = c.MemberId,
                    CheckInTime = c.CheckInTime,
                    Note = c.Note
                });
            }

            return View(result);
        }
    }
}
