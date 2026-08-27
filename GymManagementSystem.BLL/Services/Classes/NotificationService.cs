using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Notifications;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSmsService _emailSms;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUnitOfWork unitOfWork, IEmailSmsService emailSms, ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailSms = emailSms;
            _logger = logger;
        }

        public async Task NotifyAsync(string recipient, string title, string message, string type, CancellationToken ct = default)
        {
            var notification = new Notification
            {
                UserId = string.IsNullOrWhiteSpace(recipient) ? null : recipient,
                Title = title,
                Message = message,
                Type = type
            };

            _unitOfWork.GetRepositories<Notification>().Add(notification);
            await _unitOfWork.SaveChangesAsync(ct);

            if (!string.IsNullOrWhiteSpace(recipient) && recipient.Contains('@'))
            {
                await _emailSms.SendEmailAsync(recipient, title, message, ct);
            }

            _logger.LogInformation("Notification created for {Recipient}: {Title}", recipient, title);
        }

        public async Task<IEnumerable<NotificationViewModel>> GetForUserAsync(string userName, bool isAdmin, CancellationToken ct = default)
        {
            var all = await _unitOfWork.GetRepositories<Notification>().GetAllAsync(tracking: false, ct: ct);
            var query = all.AsEnumerable();

            if (!isAdmin)
            {
                query = query.Where(n => n.UserId == userName || string.IsNullOrWhiteSpace(n.UserId));
            }

            return query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToList();
        }

        public async Task<int> GetUnreadCountAsync(string userName, bool isAdmin, CancellationToken ct = default)
        {
            var all = await _unitOfWork.GetRepositories<Notification>().GetAllAsync(tracking: false, ct: ct);
            var query = all.AsEnumerable();

            if (!isAdmin)
            {
                query = query.Where(n => n.UserId == userName || string.IsNullOrWhiteSpace(n.UserId));
            }

            return query.Count(n => !n.IsRead);
        }

        public async Task MarkReadAsync(int id, string userName, bool isAdmin, CancellationToken ct = default)
        {
            var notification = await _unitOfWork.GetRepositories<Notification>().GetByIdAsync(id, ct);
            if (notification is null) return;

            // Ownership guard: members may only mark their own or global notifications as read.
            if (!isAdmin && !string.Equals(notification.UserId, userName, StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(notification.UserId))
            {
                return;
            }

            notification.IsRead = true;
            _unitOfWork.GetRepositories<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
