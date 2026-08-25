using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.ViewComponents
{
    public class NotificationBellViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationBellViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userName = HttpContext.User?.Identity?.Name ?? string.Empty;
            var isAdmin = HttpContext.User?.IsInRole("SuperAdmin") ?? false;
            var count = await _notificationService.GetUnreadCountAsync(userName, isAdmin);
            return View(count);
        }
    }
}
