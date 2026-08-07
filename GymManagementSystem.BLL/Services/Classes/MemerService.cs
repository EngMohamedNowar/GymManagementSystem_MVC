using AutoMapper;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Members;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork,IMapper mapper,IAttachmentService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }
        public async Task<bool> CreateMemberAsync(CreateMemberDTOs model, CancellationToken ct = default)
        {
            if (model.Photo == null)
            {
                throw new Exception("Photo is null");
            }

            string? fileName = null;

            try
            {
                var emailExist = await _unitOfWork
                    .GetRepositories<Member>()
                    .AnyAsync(m => m.Email == model.Email, ct);

                var phoneExist = await _unitOfWork
                    .GetRepositories<Member>()
                    .AnyAsync(m => m.Phone == model.Phone, ct);

                if (emailExist || phoneExist)
                    return false;


                // رفع الصورة
                using (var stream = model.Photo.OpenReadStream())
                {
                    fileName = await _attachmentService.UploadAsync(
                        stream,
                        "MembersPicture",
                        model.Photo.FileName,
                        ct
                    );
                }


                if (string.IsNullOrWhiteSpace(fileName))
                    return false;


                var member = _mapper.Map<Member>(model);

                member.Photo = fileName;


                _unitOfWork.GetRepositories<Member>().Add(member);

                var count = await _unitOfWork.SaveChanegesAsync(ct);

                if (count <= 0)
                {
                    // لو فشل الحفظ نحذف الصورة التي تم رفعها
                    _attachmentService.Delete("MembersPicture", fileName);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                // لو حصل Exception بعد رفع الصورة
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    _attachmentService.Delete("MembersPicture", fileName);
                }

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                throw;
            }
        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>()
                .GetByIdAsync(memberId);

            if (member == null)
                return false;

            var hasFutureSession = await _unitOfWork.GetRepositories<Booking>()
                .AnyAsync(m => m.MemberId == memberId && m.Session.StartDate > DateTime.Now, ct);

            if (hasFutureSession)
                return false;


            // حذف صورة العضو من الملفات
            if (!string.IsNullOrWhiteSpace(member.Photo))
            {
                var deleted = _attachmentService.Delete("MemberPicture", member.Photo);

                if (!deleted)
                {
                    // اختياري: لو عايز توقف الحذف لو الصورة فشلت
                    // return false;
                }
            }


            _unitOfWork.GetRepositories<Member>().Delete(member);

            var count = await _unitOfWork.SaveChanegesAsync(ct);

            return count > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepositories<Member>().GetAllAsync(ct: ct);
            if (members is null) return null;

            // Mapping --> Casting 
            //var membersDTOs = members.Select(member => new MemberViewModel()
            //{
            //    Id = member.Id,
            //    Photo = member.Photo,
            //    Name = member.Name,
            //    Phone = member.Phone,
            //    Gender = member.Gender.ToString(),
            //    Email = member.Email
            //});
            var membersDTOs = _mapper.Map<IEnumerable<MemberViewModel>>(members);
            return membersDTOs;
        }

        public async Task<MemberViewModel?> GetMemberDetalisAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct); // Member Entity
            if (member is null) return null;
            // Mapping --> Casting 
            //var membersDTOs = new MemberViewModel()
            //{
            //    Photo = member.Photo,
            //    Name = member.Name,
            //    Phone = member.Phone,
            //    Gender = member.Gender.ToString(),
            //    Email = member.Email,
            //    DateOfBirth = member.DateOfBirth.ToString(),
            //    Address = $"Building Number: {member.Address.BuildingNumber} ,Street:{member.Address.Street}, City:{member.Address.City}"
            //};
            var memberDTOs = _mapper.Map<MemberViewModel>(member);
            var activePlans = await _unitOfWork.GetRepositories<MemberShip>().FirstOrDefaultAsync(
              m => m.Id == member.Id && m.EndDate > DateTime.Now, ct);

            if(activePlans is not null)
            {
                memberDTOs.PlanName = activePlans.Plan.Name;
                memberDTOs.MemberShipEndDate = activePlans.EndDate.ToString();
                memberDTOs.MemberShipStartDate = activePlans.CreatedAt.ToString();    
            }
            return memberDTOs;
        }

        public async Task<HealthRecordViewModel?> GetMemberHelthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var memberHealthRecord = await _unitOfWork.GetRepositories<HealthRecord>().FirstOrDefaultAsync(m => m.Id == memberId,ct);
            if (memberHealthRecord is null) return null;
            //var healthRecord = new HealthRecordViewModel()
            //{
            //    Height = memberHealthRecord.Height,
            //    Note = memberHealthRecord.Note,
            //    BloodType = memberHealthRecord.BloodType,
            //    Weight = memberHealthRecord.weight
            //};
            var healthRecord = _mapper.Map<HealthRecordViewModel>(memberHealthRecord);
            return healthRecord;
        }

        public async Task<UpdateMemberDTOs> MemberToUpdateAsync(int meberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(meberId, ct);
            if (member is null) return null;
            //var memberUpdated = new UpdateMemberDTOs()
            //{
            //    Photo = member.Photo,
            //    Name = member.Name,
            //    Email =member.Email,
            //    Phone =member.Phone,
            //    DateOfBirth = member.DateOfBirth,
            //    BuildingNumber = member.Address.BuildingNumber,
            //    City = member.Address.City,
            //    Street = member.Address.Street
            //};
            var memberUpdated = _mapper.Map<UpdateMemberDTOs>(member);
            return memberUpdated;
        }

        public async Task<bool> UpdateMemberAsync(int memberId, UpdateMemberDTOs model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId,ct);
            if (member == null) return false;

            var emailExist = await _unitOfWork.GetRepositories<Member>().AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct);
            var phoneExist = await _unitOfWork.GetRepositories<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);

            if (emailExist || phoneExist) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            _unitOfWork.GetRepositories<Member>().Update(member);
            var count = await _unitOfWork.SaveChanegesAsync(ct);
            return count > 0;
        }
    }
}
