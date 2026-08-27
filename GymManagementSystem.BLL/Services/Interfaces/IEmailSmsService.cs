using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IEmailSmsService
    {
        bool IsEmailConfigured { get; }
        Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
        Task SendSmsAsync(string to, string message, CancellationToken ct = default);
    }
}
