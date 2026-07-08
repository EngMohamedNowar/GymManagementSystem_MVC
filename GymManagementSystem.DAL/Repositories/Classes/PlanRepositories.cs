using GymManagement.DbContexts;
using GymManagement.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class PlanRepositories : IPlanRepositories
    {
        private readonly GymDbContext _context;
        public PlanRepositories(GymDbContext Context)
        {
            _context = Context;
        }

        public async Task<int> AddAsync(Plan plan, CancellationToken ct = default)
        {
           await _context.Plans.AddAsync(plan, ct);
           return await _context.SaveChangesAsync();
        }

        public async Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default)

           => await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);


        public async Task<int> UpdateAsync(Plan plan, CancellationToken ct = default)
        {
            _context.Plans.Update(plan);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken ct = default)
        {
            _context.Plans.Remove(plan);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Plan>> GetAllPlansAsync(bool tracking = false, CancellationToken ct = default)
           => tracking? await _context.Plans.ToListAsync(ct): await _context.Plans.AsNoTracking().ToListAsync(ct);


    }
}
