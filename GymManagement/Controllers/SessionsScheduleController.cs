using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class SessionsScheduleController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IBookingService _bookingService;

        public SessionsScheduleController(ISessionService sessionService, IBookingService bookingService)
        {
            _sessionService = sessionService;
            _bookingService = bookingService;
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
        public async Task<IActionResult> ToggleAttendance(int sessionId, int memberId, CancellationToken ct)
        {
            var result = await _bookingService.ToggleAttendanceAsync(sessionId, memberId, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            return RedirectToAction(nameof(Attendees), new { id = sessionId });
        }
    }
}
