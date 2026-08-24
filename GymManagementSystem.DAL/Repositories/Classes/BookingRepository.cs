using GymManagement.DbContexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepositories<Booking>, IBookingRepository
    {
        private readonly GymDbContext _context;

        public BookingRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(CancellationToken ct = default)
        {
            var bookings = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Member)
                .Include(b => b.Session).ThenInclude(s => s.Trainer)
                .Include(b => b.Session).ThenInclude(s => s.Category)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(ct);
            return bookings;
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Member)
                .Include(b => b.Session).ThenInclude(s => s.Trainer)
                .Include(b => b.Session).ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
            return booking;
        }

        // Tracked fetch (no AsNoTracking) so it can be mutated and saved directly.
        public async Task<Booking?> GetBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
            return booking;
        }

        public async Task<bool> ExistsBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            return await _context.Bookings.AnyAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
        }
    }
}
