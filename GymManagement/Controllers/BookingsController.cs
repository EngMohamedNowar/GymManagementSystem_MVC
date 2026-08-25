using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Bookings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var bookings = await _bookingService.GetAllBookingsAsync(ct);
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Members = new SelectList(await _bookingService.GetAllMembersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Sessions = new SelectList(await _bookingService.GetAvailableSessionsForDropDownAsync(ct), "Id", "Display");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                var result = await _bookingService.CreateBookingAsync(model, ct);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "Booking Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["FailedMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Members = new SelectList(await _bookingService.GetAllMembersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Sessions = new SelectList(await _bookingService.GetAvailableSessionsForDropDownAsync(ct), "Id", "Display");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int sessionId, int memberId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(sessionId, memberId, ct);

            TempData["FlashMessage"] = result.error ?? "Booking Cancelled Successfully";
            TempData["FlashSuccess"] = result.success;

            return RedirectToAction(nameof(Index));
        }
    }
}
