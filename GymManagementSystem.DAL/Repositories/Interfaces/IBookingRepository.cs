using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepositories<Booking>
    {
        Task<IEnumerable<Booking>> GetAllBookingsWithDetailsAsync(CancellationToken ct = default);
        Task<Booking?> GetBookingWithDetailsAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Booking?> GetBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<bool> ExistsBookingAsync(int memberId, int sessionId, CancellationToken ct = default);
    }
}
