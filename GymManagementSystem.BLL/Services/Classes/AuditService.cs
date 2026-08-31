using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Audit;
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
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuditService> _logger;

        public AuditService(IUnitOfWork unitOfWork, ILogger<AuditService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task LogAsync(string? userName, string action, string entity, string? entityId, string? details, CancellationToken ct = default)
        {
            try
            {
                var log = new AuditLog
                {
                    UserName = userName ?? "Anonymous",
                    Action = action,
                    Entity = entity,
                    EntityId = entityId,
                    Details = details
                };

                _unitOfWork.GetRepositories<AuditLog>().Add(log);
                await _unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write audit log for {Action}", action);
            }
        }

        public async Task<IEnumerable<AuditLogViewModel>> GetRecentAsync(int count, CancellationToken ct = default)
        {
            var all = await _unitOfWork.GetRepositories<AuditLog>().GetAllAsync(tracking: false, ct: ct);
            return all
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .Select(a => new AuditLogViewModel
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Action = a.Action,
                    Entity = a.Entity,
                    EntityId = a.EntityId,
                    Details = a.Details,
                    Timestamp = a.CreatedAt
                })
                .ToList();
        }
    }
}
