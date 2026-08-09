using GymManagement.DbContexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class MembershipRepository : GenericRepositories<MemberShip>, IMembershipRepository
    {
        private readonly GymDbContext _context;

        public MembershipRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MemberShip>> GetAllMembershipsWithDetailsAsync(CancellationToken ct = default)
        {
            var memberships = await _context.MemberShips
                .AsNoTracking()
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
            return memberships;
        }

        public async Task<MemberShip?> GetMembershipByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            var membership = await _context.MemberShips
                .AsNoTracking()
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .FirstOrDefaultAsync(m => m.Id == id, ct);
            return membership;
        }

        public async Task<MemberShip?> GetActiveMembershipByMemberIdAsync(int memberId, CancellationToken ct = default)
        {
            var membership = await _context.MemberShips
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.UtcNow, ct);
            return membership;
        }
    }
}
