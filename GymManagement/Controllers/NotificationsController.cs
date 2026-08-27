using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var isAdmin = User.IsInRole("SuperAdmin");
            var notifications = await _notificationService.GetForUserAsync(userName, isAdmin, ct);
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
        {
            var userName = User.Identity?.Name ?? string.Empty;
            var isAdmin = User.IsInRole("SuperAdmin");
            await _notificationService.MarkReadAsync(id, userName, isAdmin, ct);
            return RedirectToAction(nameof(Index));
        }
    }
}
