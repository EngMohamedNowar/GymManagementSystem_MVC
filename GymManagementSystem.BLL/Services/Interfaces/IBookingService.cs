using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingViewModel>> GetAllBookingsAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<SessionSelectViewModel>> GetAvailableSessionsForDropDownAsync(CancellationToken ct = default);
        Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default);
        Task<Result> CancelBookingAsync(int sessionId, int memberId, CancellationToken ct = default);
        Task<Result> ToggleAttendanceAsync(int sessionId, int memberId, CancellationToken ct = default);
    }
}
