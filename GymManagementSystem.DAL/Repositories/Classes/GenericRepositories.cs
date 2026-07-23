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

        public async Task<int> AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity,ct);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _context.Set<TEntity>().AnyAsync(predicate, ct =default);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(predicate, ct = default);

        }
    }
}
 