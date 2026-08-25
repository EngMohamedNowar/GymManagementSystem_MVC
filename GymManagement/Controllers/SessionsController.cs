using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.BLL.ViewModels.Sessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]

    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IAuditService _auditService;

        public SessionsController(ISessionService sessionService, IAuditService auditService)
        {
            _sessionService = sessionService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            var all = (await _sessionService.GetAllSessionsAsync(ct: ct))?.ToList() ?? new List<SessionViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                all = all.Where(se =>
                    (se.CategoryName?.ToLower().Contains(s) ?? false) ||
                    (se.Description?.ToLower().Contains(s) ?? false) ||
                    (se.TrainerName?.ToLower().Contains(s) ?? false)).ToList();
            }

            var paged = new PaginationViewModel<SessionViewModel>
            {
                Items = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = all.Count
            };

            ViewBag.Search = search;
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionDetailsById(id, ct);
            if (result.success)
            {
                return View(result.value);
            }
            TempData["ErrorMessage"] = result.error;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);

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
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, ct);

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
            if (ModelState.IsValid)
            {
                var result = await _sessionService.CreateSessionAsync(model, ct);
                if (result.success)
                {
                    TempData["SuccessMessage"] = "session Created Successfully";
                    await _auditService.LogAsync(User.Identity?.Name, "Create Session", "Session", null, model.Description, ct);
                }
                else
                {
                    TempData["FailedMessage"] = result.error;
                }
                return RedirectToAction("Index");
            }
            ViewBag.Trainers = new SelectList(await _sessionService.GetAllTrainersForDropDownAsync(ct), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetAllCategoriesForDropDownAsync(ct), "Id", "Name");
            return View(model);
        }
    }
}
