using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MembershipExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MembershipExpiryService> _logger;
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

        public MembershipExpiryService(IServiceScopeFactory scopeFactory, ILogger<MembershipExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                await ProcessExpiredAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Initial membership expiry pass failed.");
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!await timer.WaitForNextTickAsync(stoppingToken))
                        break;

                    await ProcessExpiredAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Membership expiry pass failed.");
                }
            }
        }

        private async Task ProcessExpiredAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var memberships = await unitOfWork.GetRepositories<MemberShip>().GetAllAsync(tracking: true, ct: ct);
            var now = DateTime.UtcNow;
            var changed = false;

            foreach (var membership in memberships)
            {
                if (membership.EndDate < now && membership.Status != "Expired")
                {
                    membership.Status = "Expired";
                    membership.UpdatedAt = now;
                    unitOfWork.GetRepositories<MemberShip>().Update(membership);
                    changed = true;

                    var member = await unitOfWork.GetRepositories<Member>().GetByIdAsync(membership.MemberId, ct);
                    await notificationService.NotifyAsync(
                        member?.Email ?? "system",
                        "Membership Expired",
                        $"The membership for {member?.Name ?? "a member"} has expired. Please renew to keep access.",
                        "Renewal",
                        ct);
                }
            }

            if (changed)
            {
                await unitOfWork.SaveChangesAsync(ct);
                _logger.LogInformation("Membership expiry service marked expired memberships.");
            }
        }
    }
}
