using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AuditController : Controller
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var logs = await _auditService.GetRecentAsync(200, ct);
            return View(logs);
        }
    }
}
