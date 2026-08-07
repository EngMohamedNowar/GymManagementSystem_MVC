using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Members;
using GymManagementSystem.BLL.ViewModes.Trainers;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService,IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetalisAsync(id, ct);

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
            if (!ModelState.IsValid)   // اتأكد الأول
            {
                return View(model);   // لو غلط، ارجع بدون ما تنفذ الإضافة خالص
            }

            var result = await _memberService.CreateMemberAsync(model, ct); // ينفذ بس لو صح
            if (result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
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
            var memberDetails = await _memberService.GetMemberDetalisAsync(id, ct);
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
            var memberDetails = await _memberService.GetMemberHelthRecordAsync(id, ct);
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
        public async Task<IActionResult> Delete(int id,CancellationToken ct)
        {
            var member = _memberService.GetMemberDetalisAsync(id);
            if(member is null)
            {
                TempData["Error Message"] = "Member Not Found";
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
                //return RedirectToAction("Index");
            }
            else
            {
                TempData["FailedMessage"] = "Failed To Deleted Member";
            }
            return RedirectToAction("Index");

        }
    }
}
