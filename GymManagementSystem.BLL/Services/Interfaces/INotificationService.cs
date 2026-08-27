using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Notifications;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAsync(string recipient, string title, string message, string type, CancellationToken ct = default);
        Task<IEnumerable<NotificationViewModel>> GetForUserAsync(string userName, bool isAdmin, CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(string userName, bool isAdmin, CancellationToken ct = default);
        Task MarkReadAsync(int id, string userName, bool isAdmin, CancellationToken ct = default);
    }
}
