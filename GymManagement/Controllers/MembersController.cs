using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Common;
using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAuditService _auditService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService, IAuditService auditService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
            _auditService = auditService;
        }
        public async Task<IActionResult> Index(string? search, string? gender, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            page = page < 1 ? 1 : page;
            var all = (await _memberService.GetAllMembersAsync(ct))?.ToList() ?? new List<MemberViewModel>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                all = all.Where(m =>
                    (m.Name?.ToLower().Contains(s) ?? false) ||
                    (m.Email?.ToLower().Contains(s) ?? false) ||
                    (m.Phone?.ToLower().Contains(s) ?? false)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(gender))
            {
                all = all.Where(m => string.Equals(m.Gender, gender, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var paged = new PaginationViewModel<MemberViewModel>
            {
                Items = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = all.Count
            };

            ViewBag.Search = search;
            ViewBag.Gender = gender;
            return View(paged);
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);

            if (member is null || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound();

            var result = _attachmentService.GetFile("MembersPicture", member.Photo);

            if (result is null)
                return NotFound();

            return File(result.Value.stream, result.Value.contentType);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateMemberDTOs());

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberDTOs model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result > 0)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Create Member", "Member", result.ToString(), model.Email, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Create Member";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct = default)
        {
            var memberDetails = await _memberService.GetMemberDetailsAsync(id, ct);
            if (memberDetails is null)
            {
                TempData["FailedMessage"] = "Member Not Found";
                return RedirectToAction("Index");

            }
            return View(memberDetails);
        }

        [HttpGet]
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var memberDetails = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (memberDetails is null)
            {
                TempData["FailedMessage"] = "Member Not Found";
                return RedirectToAction("Index");

            }
            return View(memberDetails);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var memberUpdated = await _memberService.MemberToUpdateAsync(id, ct);
            if (memberUpdated is null)
            {
                TempData["FailedMessage"] = "Member Not Found";
                return RedirectToAction("Index");

            }
            return View(memberUpdated);
        }

        [HttpPost]
        public async Task<IActionResult> EditMember(int id, UpdateMemberDTOs model, CancellationToken ct)
        {
            if (ModelState.IsValid)
            {
                var memberUpdated = await _memberService.UpdateMemberAsync(id, model, ct);

                if (memberUpdated)
                {
                    TempData["SuccessMessage"] = "Member Updated Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["FailedMessage"] = "Failed To Update Member";
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsAsync(id, ct);
            if (member is null)
            {
                TempData["FailedMessage"] = "Member Not Found";
                return RedirectToAction("Index");
            }
            return View(member);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var member = await _memberService.DeleteMemberAsync(id);

            if (member)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully";
                await _auditService.LogAsync(User.Identity?.Name, "Delete Member", "Member", id.ToString(), null, ct);
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Deleted Member";
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv(CancellationToken ct)
        {
            var all = (await _memberService.GetAllMembersAsync(ct))?.ToList() ?? new List<MemberViewModel>();

            var sb = new StringBuilder();
            sb.AppendLine("Id,Name,Email,Phone,Gender,Plan,MembershipStart,MembershipEnd");

            foreach (var m in all)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    Csv(m.Id.ToString()),
                    Csv(m.Name),
                    Csv(m.Email),
                    Csv(m.Phone),
                    Csv(m.Gender),
                    Csv(m.PlanName),
                    Csv(m.MemberShipStartDate),
                    Csv(m.MemberShipEndDate)
                }));
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "members.csv");
        }

        private static string Csv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "\"\"";
            var escaped = value.Replace("\"", "\"\"");
            return $"\"{escaped}\"";
        }
    }
}
