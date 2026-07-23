using GymManagement.Models;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepositories<TEntity> where TEntity : Base , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<int> AddAsync(TEntity entity, CancellationToken ct = default);
        Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct);

    }
}
