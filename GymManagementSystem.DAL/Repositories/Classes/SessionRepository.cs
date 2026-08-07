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
    public class SessionRepository : GenericRepositories<Session>, ISessionRepository 
    {
        private readonly GymDbContext _context;


        public SessionRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Session>> GetAllSessionWithTrainerAndCategoryAsync(CancellationToken ct)
        {
         var sessions = await _context.Sessions.AsNoTracking().Include(m => m.Trainer).Include(m => m.Category).ToArrayAsync(ct);
            return sessions;
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct)
        {
         var result = await _context.Bookings.CountAsync(s => s.SessionId == sessionId);
            return result;
        }

        public  async Task<Session?> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
         var session = await _context.Sessions.AsNoTracking().Include(s => s.Trainer).Include(s => s.Category).FirstOrDefaultAsync(s=>s.Id == sessionId);
            return session;
        }
    }
} 
