using GymManagement.DbContexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepositories<TEntity> : IGenericRepositories<TEntity> where TEntity : Base, new()
    {
        private readonly GymDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepositories(GymDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
            => tracking ? await _dbSet.ToListAsync(ct) : await _context.Set<TEntity>().AsNoTracking().ToListAsync(ct);

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
           => await _dbSet.FindAsync(id,ct);

        public void Add(TEntity entity)
        {
             _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, ct);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, ct);

        }

        // Overload بياخد شرط (lambda)
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        => predicate is null ? await _context.Set<TEntity>().AsNoTracking().CountAsync(ct) : await _context.Set<TEntity>().AsNoTracking().CountAsync(predicate, ct);
    }
}
 