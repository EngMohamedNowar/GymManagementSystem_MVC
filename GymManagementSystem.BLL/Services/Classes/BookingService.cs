using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Bookings;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookingViewModel>> GetAllBookingsAsync(CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.bookingRepository.GetAllBookingsWithDetailsAsync(ct);
            if (bookings is null) return null;

            var bookingsDTOs = bookings.Select(b => new BookingViewModel()
            {
                MemberId = b.MemberId,
                MemberName = b.Member.Name,
                MemberPhone = b.Member.Phone,
                SessionId = b.SessionId,
                SessionCategoryName = b.Session.Category.Name,
                TrainerName = b.Session.Trainer.Name,
                SessionStartDate = b.Session.StartDate,
                SessionEndDate = b.Session.EndDate,
                IsAttended = b.IsAttended,
                BookedAt = b.CreatedAt
            }).ToList();

            return bookingsDTOs;
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepositories<Member>().GetAllAsync(ct: ct);
            if (members is null) return null;

            var membersDTOs = members.Select(m => new MemberSelectViewModel()
            {
                Id = m.Id,
                Name = m.Name
            });

            return membersDTOs;
        }

        public async Task<IEnumerable<SessionSelectViewModel>> GetAvailableSessionsForDropDownAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.sessionRepository.GetAllSessionWithTrainerAndCategoryAsync(ct);
            if (sessions is null) return null;

            var result = new List<SessionSelectViewModel>();
            foreach (var session in sessions.Where(s => s.StartDate > DateTime.Now).OrderBy(s => s.StartDate))
            {
                var bookedCount = await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
                var availableSlots = session.Capacity - bookedCount;
                if (availableSlots <= 0) continue;

                result.Add(new SessionSelectViewModel()
                {
                    Id = session.Id,
                    Display = $"{session.Category.Name} - {session.StartDate:MMM dd, yyyy hh:mm tt} with {session.Trainer.Name} ({availableSlots} slots left)"
                });
            }

            return result;
        }

        public async Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(model.MemberId, ct);
            if (member is null) return Result.NotFound($"Member with id {model.MemberId} not found");

            var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(model.SessionId, ct);
            if (session is null) return Result.NotFound($"Session with id {model.SessionId} not found");

            if (session.StartDate <= DateTime.Now) return Result.Validation("Cannot book a session that has already started");

            var activeMembership = await _unitOfWork.membershipRepository.GetActiveMembershipByMemberIdAsync(model.MemberId, ct);
            if (activeMembership is null) return Result.Validation("Member must have an active membership to book a session");

            var alreadyBooked = await _unitOfWork.bookingRepository.ExistsBookingAsync(model.MemberId, model.SessionId, ct);
            if (alreadyBooked) return Result.Fail("This member already has a booking for this session");

            var bookedCount = await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (bookedCount >= session.Capacity) return Result.Fail("This session is fully booked");

            var booking = new Booking()
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false
            };

            _unitOfWork.bookingRepository.Add(booking);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to create booking");
        }

        public async Task<Result> CancelBookingAsync(int sessionId, int memberId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.bookingRepository.GetBookingAsync(memberId, sessionId, ct);
            if (booking is null) return Result.NotFound("Booking not found");

            var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(sessionId, ct);
            if (session is not null && session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that has already started");

            _unitOfWork.bookingRepository.Delete(booking);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to cancel booking");
        }

        public async Task<Result> ToggleAttendanceAsync(int sessionId, int memberId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.bookingRepository.GetBookingAsync(memberId, sessionId, ct);
            if (booking is null) return Result.NotFound("Booking not found");

            var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session not found");

            if (session.StartDate > DateTime.Now) return Result.Validation("Cannot mark attendance before the session starts");

            booking.IsAttended = !booking.IsAttended;
            booking.UpdatedAt = DateTime.Now;

            _unitOfWork.bookingRepository.Update(booking);
            var count = await _unitOfWork.SaveChangesAsync(ct);

            return count > 0 ? Result.Ok() : Result.Fail("Failed to update attendance");
        }
    }
}
