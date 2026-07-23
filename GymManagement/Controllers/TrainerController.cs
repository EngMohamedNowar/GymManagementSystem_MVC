using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Trainers;
using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class TrainersController: Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetAllTrainersAsync(ct);
            return View(trainers);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateTrainerDTOs());

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerDTOs model, CancellationToken ct)
        {
            if (!ModelState.IsValid)   // اتأكد الأول
            {
                return View(model);   // لو غلط، ارجع بدون ما تنفذ الإضافة خالص
            }

            var result = await _trainerService.CreateTrainerAsync(model, ct); // ينفذ بس لو صح
            if (result)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully";
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
            var trainerDetails = await _trainerService.GetTrainerDetalisAsync(id, ct);
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
            var trainerUpdated = await _trainerService.TrainerToUpdateAsync(id,ct);
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
            var trainer = await _trainerService.GetTrainerDetalisAsync(id, ct);
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
            var trainer = await _trainerService.DeleteTrainerAsync(id,ct);

            if (trainer)
            {
                TempData["SuccessMessage"] = "trainer Deleted Successfully";
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Deleted trainer";
            }
            return RedirectToAction("Index");

        }

    }
}
