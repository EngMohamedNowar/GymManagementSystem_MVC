using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class SessionsScheduleController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsScheduleController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var sessions = await _sessionService.GetSessionsScheduleAsync(ct);
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Attendees(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionAttendeesAsync(id, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SessionId = id;
            return View(result.value);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int memberId, int sessionId, bool isAttended, CancellationToken ct)
        {
            var result = await _sessionService.SetAttendanceAsync(sessionId, memberId, isAttended, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction(nameof(Attendees), new { id = sessionId });
        }
    }
}
