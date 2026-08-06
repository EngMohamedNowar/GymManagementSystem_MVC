using GymManagement.Models;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepositories<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionWithTrainerAndCategoryAsync(CancellationToken cancellationToken);
        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct);
        Task<Session?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default);

    }
}
