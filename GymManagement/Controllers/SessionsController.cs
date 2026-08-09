using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Members;
using GymManagementSystem.BLL.ViewModes.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var session = await _sessionService.GetAllSessionsAsync(ct: ct);

            return View(session); 
        }




        [HttpGet]
        public async Task<IActionResult>Details(int id,CancellationToken ct)
        {
            var result =await _sessionService.GetSessionDetailsById(id,ct);
            if (result.success)
            {
                return View(result.value);
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult>Edit(int id,CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id,ct);

            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _sessionService.UpdateSessionAsync(id,model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "session Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
                TempData["ErrorMessage"] = result.error;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetAllCategoriesForDropDownAsync(ct), "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (ModelState.IsValid)   // اتأكد الأول
            {
                var result = await _sessionService.CreateSessionAsync(model, ct); // ينفذ بس لو صح
                if (result.success)
                {
                    TempData["SuccessMessage"] = "session Created Successfully";
                }
                else
                {
                    TempData["FailedMessage"] = result.error;
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }


    } 
}
