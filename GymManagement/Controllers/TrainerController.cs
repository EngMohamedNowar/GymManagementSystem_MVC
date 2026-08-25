using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Common;
using GymManagementSystem.BLL.ViewModels.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IAuditService _auditService;

        public TrainersController(ITrainerService trainerService, IAuditService auditService)
        {
            _trainerService = trainerService;
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            var all = (await _trainerService.GetAllTrainersAsync(ct))?.ToList() ?? new List<TrainerViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                all = all.Where(t =>
                    (t.Name?.ToLower().Contains(s) ?? false) ||
                    t.Specialization.ToString().ToLower().Contains(s) ||
                    (t.Email?.ToLower().Contains(s) ?? false)).ToList();
            }

            var paged = new PaginationViewModel<TrainerViewModel>
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
        public IActionResult Create() => View(new CreateTrainerDTOs());

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerDTOs model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _trainerService.CreateTrainerAsync(model, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Create Trainer", "Trainer", null, model.Email, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Create Trainer";
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var trainerDetails = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainerDetails is null)
            {
                TempData["FailedMessage"] = "Trainer Not Found";
                return RedirectToAction("Index");

            }
            return View(trainerDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainerUpdated = await _trainerService.TrainerToUpdateAsync(id, ct);
            if (trainerUpdated is null)
            {
                TempData["FailedMessage"] = "Trainer Not Found";
                return RedirectToAction("Index");

            }
            return View(trainerUpdated);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateTrainerDTOs model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                var trainerUpdated = await _trainerService.UpdateTrainerAsync(id, model, ct);

                if (trainerUpdated)
                {
                    TempData["SuccessMessage"] = "Trainer Updated Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["FailedMessage"] = "Failed To Update Trainer";
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["FailedMessage"] = "Trainer Not Found";
                return RedirectToAction("Index");
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.DeleteTrainerAsync(id, ct);

            if (trainer)
            {
                TempData["SuccessMessage"] = "trainer Deleted Successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Delete Trainer", "Trainer", id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Deleted trainer";
            }
            return RedirectToAction("Index");

        }
    }
}
