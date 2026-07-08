using GymManagement.DbContexts;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    public class PlansController :Controller
    {

        private readonly IPlanRepositories _planRepositories;
        public PlansController(IPlanRepositories planRepositories)
        {
            _planRepositories = planRepositories;
        }
        // Index
        public async Task<IActionResult> Index(CancellationToken ct= default)
        {
            //var plans= await _Context.Plans.ToListAsync();
            var plans = await _planRepositories.GetAllPlansAsync(ct:ct);

            return View(plans); // name inedex .cshtml
        }
        public async Task<IActionResult> Details(int id,CancellationToken ct = default)
        {
            //var plan = await _Context.Plans.FirstOrDefaultAsync(p => p.Id == id);
            var plan = await _planRepositories.GetByIdAsync(id,ct:ct);

            if (plan is null) return RedirectToAction(nameof(Index));
            return View(plan); // name details.cshtml
        }
        // Details
    }
}
