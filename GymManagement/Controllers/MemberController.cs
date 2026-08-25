using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Memberships;
using GymManagementSystem.BLL.ViewModels.Payments;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "Member,SuperAdmin,Admin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IMembershipService _membershipService;
        private readonly IBodyMeasurementService _bodyMeasurementService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;

        public MemberController(IMemberService memberService,
            IMembershipService membershipService,
            IBodyMeasurementService bodyMeasurementService,
            IUnitOfWork unitOfWork,
            IAuditService auditService)
        {
            _memberService = memberService;
            _membershipService = membershipService;
            _bodyMeasurementService = bodyMeasurementService;
            _unitOfWork = unitOfWork;
            _auditService = auditService;
        }

        private async Task<MemberViewModel?> GetCurrentMemberAsync(CancellationToken ct)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _memberService.GetMemberByEmailAsync(email, ct);
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return View((MemberViewModel?)null);
            }

            var all = (await _membershipService.GetAllAsync(ct))?.ToList() ?? new List<MembershipViewModel>();
            var myMemberships = all.Where(m => m.MemberId == member.Id).ToList();
            ViewBag.Memberships = myMemberships;
            ViewBag.Payments = myMemberships.Any()
                ? await _membershipService.GetPaymentsByMembershipAsync(myMemberships.First().Id, ct)
                : Enumerable.Empty<PaymentViewModel>();
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> MyMembership(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }

            var all = (await _membershipService.GetAllAsync(ct))?.ToList() ?? new List<MembershipViewModel>();
            var myMemberships = all.Where(m => m.MemberId == member.Id).ToList();
            ViewBag.Member = member;
            ViewBag.Payments = await _membershipService.GetAllPaymentsAsync(ct);
            return View(myMemberships);
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }

            var model = await _memberService.GetMemberProfileAsync(member.Id, ct);
            if (model is null) return RedirectToAction(nameof(Index));
            model.Id = member.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MyProfile(MemberProfileEditViewModel model, CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }

            model.Id = member.Id;

            if (!ModelState.IsValid)
                return View(model);

            var updated = await _memberService.UpdateMemberProfileAsync(member.Id, model, ct);
            if (updated)
            {
                TempData["SuccessMessage"] = "Profile updated successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Update Own Profile", "Member", member.Id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed to update profile";
            }

            return RedirectToAction(nameof(MyProfile));
        }

        [HttpGet]
        public async Task<IActionResult> MyBookings(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }

            var bookings = await _unitOfWork.GetRepositories<Booking>().GetAllAsync(tracking: false, ct: ct);
            var myBookings = bookings.Where(b => b.MemberId == member.Id).ToList();

            var result = new List<MemberBookingViewModel>();
            foreach (var b in myBookings)
            {
                var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(b.SessionId, ct);
                result.Add(new MemberBookingViewModel
                {
                    BookingId = b.Id,
                    SessionId = b.SessionId,
                    SessionTitle = session?.Description ?? "—",
                    StartDate = session?.StartDate ?? DateTime.MinValue,
                    EndDate = session?.EndDate ?? DateTime.MinValue,
                    IsAttended = b.IsAttended,
                    CanCancel = session is not null && session.StartDate > DateTime.UtcNow
                });
            }

            result = result.OrderBy(r => r.StartDate).ToList();
            ViewBag.Member = member;
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookSession(int sessionId, CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(MyBookings));
            }

            var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(sessionId, ct);
            if (session is null)
            {
                TempData["FailedMessage"] = "Session not found";
                return RedirectToAction(nameof(MyBookings));
            }

            if (session.StartDate <= DateTime.UtcNow)
            {
                TempData["FailedMessage"] = "Cannot book a session that has already started";
                return RedirectToAction(nameof(MyBookings));
            }

            var booked = await _unitOfWork.GetRepositories<Booking>().CountAsync(b => b.SessionId == sessionId, ct);
            if (booked >= session.Capacity)
            {
                TempData["FailedMessage"] = "Sorry, this session is fully booked";
                return RedirectToAction(nameof(MyBookings));
            }

            var already = await _unitOfWork.GetRepositories<Booking>().AnyAsync(b => b.SessionId == sessionId && b.MemberId == member.Id, ct);
            if (already)
            {
                TempData["FailedMessage"] = "You are already booked for this session";
                return RedirectToAction(nameof(MyBookings));
            }

            _unitOfWork.GetRepositories<Booking>().Add(new Booking
            {
                MemberId = member.Id,
                SessionId = sessionId
            });

            var count = await _unitOfWork.SaveChangesAsync(ct);
            if (count > 0)
            {
                TempData["SuccessMessage"] = "Session booked successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Book Session", "Booking", null, $"Session {sessionId}", ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed to book session";
            }

            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId, CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(MyBookings));
            }

            var booking = await _unitOfWork.GetRepositories<Booking>().GetByIdAsync(bookingId, ct);
            if (booking is null || booking.MemberId != member.Id)
            {
                TempData["FailedMessage"] = "Booking not found";
                return RedirectToAction(nameof(MyBookings));
            }

            _unitOfWork.GetRepositories<Booking>().Delete(booking);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            TempData["SuccessMessage"] = count > 0 ? "Booking cancelled" : "Failed to cancel booking";
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpGet]
        public async Task<IActionResult> MyQR(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "No member profile is linked to your account.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> QrImage(CancellationToken ct = default)
        {
            var member = await GetCurrentMemberAsync(ct);
            if (member is null) return NotFound();

            var payload = $"{Request.Scheme}://{Request.Host}/CheckIn/Scan?memberId={member.Id}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(8);

            return File(qrBytes, "image/png");
        }
    }
}
