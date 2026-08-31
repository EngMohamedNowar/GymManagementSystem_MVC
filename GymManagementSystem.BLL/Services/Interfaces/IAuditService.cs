using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GymManagementSystem.BLL.ViewModels.Audit;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string? userName, string action, string entity, string? entityId, string? details, CancellationToken ct = default);
        Task<IEnumerable<AuditLogViewModel>> GetRecentAsync(int count, CancellationToken ct = default);
    }
}
