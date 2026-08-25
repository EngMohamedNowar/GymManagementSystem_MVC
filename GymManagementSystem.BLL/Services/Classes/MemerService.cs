using AutoMapper;
using GymManagement.Models;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Members;
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
        public async Task<int> CreateMemberAsync(CreateMemberDTOs model, CancellationToken ct = default)
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
                    return 0;


                // ??? ??????
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
                    return 0;


                var member = _mapper.Map<Member>(model);

                member.Photo = fileName;


                _unitOfWork.GetRepositories<Member>().Add(member);

                var count = await _unitOfWork.SaveChangesAsync(ct);

                if (count <= 0)
                {
                    // ?? ??? ????? ???? ?????? ???? ?? ????? ????????
                    _attachmentService.Delete("MembersPicture", fileName);
                    return 0;
                }

                return member.Id;
            }
            catch (Exception ex)
            {
                // ?? ??? Exception ??? ??? ??????
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
                .GetByIdAsync(memberId, ct);

            if (member == null)
                return false;

            var hasFutureSession = await _unitOfWork.GetRepositories<Booking>()
                .AnyAsync(m => m.MemberId == memberId && m.Session.StartDate > DateTime.UtcNow, ct);

            if (hasFutureSession)
                return false;

            _unitOfWork.GetRepositories<Member>().Delete(member);

            var count = await _unitOfWork.SaveChangesAsync(ct);

            if (count <= 0)
                return false;

            // ??? ???? ????? ?? ??????? ??? ???? ????? ?? ????? ????????
            if (!string.IsNullOrWhiteSpace(member.Photo))
            {
                _attachmentService.Delete("MembersPicture", member.Photo);
            }

            return true;
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

        public async Task<MemberViewModel?> GetMemberDetailsAsync(int memberId, CancellationToken ct = default)
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
              m => m.MemberId == member.Id && m.EndDate > DateTime.UtcNow, ct);

            if(activePlans is not null)
            {
                var plan = await _unitOfWork.GetRepositories<Plan>().FirstOrDefaultAsync(p => p.Id == activePlans.PlanId, ct);
                memberDTOs.PlanName = plan?.Name;
                memberDTOs.MemberShipEndDate = activePlans.EndDate.ToString();
                memberDTOs.MemberShipStartDate = activePlans.CreatedAt.ToString();    
            }
            return memberDTOs;
        }

        public async Task<MemberViewModel?> GetMemberByEmailAsync(string email, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var member = await _unitOfWork.GetRepositories<Member>().FirstOrDefaultAsync(m => m.Email == email, ct);
            if (member is null) return null;
            var memberDTOs = _mapper.Map<MemberViewModel>(member);
            var activePlans = await _unitOfWork.GetRepositories<MemberShip>().FirstOrDefaultAsync(
              m => m.MemberId == member.Id && m.EndDate > DateTime.UtcNow, ct);
            if(activePlans is not null)
            {
                var plan = await _unitOfWork.GetRepositories<Plan>().FirstOrDefaultAsync(p => p.Id == activePlans.PlanId, ct);
                memberDTOs.PlanName = plan?.Name;
                memberDTOs.MemberShipEndDate = activePlans.EndDate.ToString();
                memberDTOs.MemberShipStartDate = activePlans.CreatedAt.ToString();    
            }
            return memberDTOs;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return null;

            var memberHealthRecord = await _unitOfWork.GetRepositories<HealthRecord>().FirstOrDefaultAsync(m => m.Id == member.HealthId,ct);
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

        public async Task<UpdateMemberDTOs> MemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
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

            member.Name = model.Name;
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            _unitOfWork.GetRepositories<Member>().Update(member);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0;
        }

        public async Task<MemberProfileEditViewModel?> GetMemberProfileAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return null;

            var health = await _unitOfWork.GetRepositories<HealthRecord>().FirstOrDefaultAsync(h => h.Id == member.HealthId, ct);

            return new MemberProfileEditViewModel
            {
                Id = member.Id,
                Phone = member.Phone,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street,
                HealthNote = health?.Note
            };
        }

        public async Task<bool> UpdateMemberProfileAsync(int memberId, MemberProfileEditViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(memberId, ct);
            if (member is null) return false;

            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            _unitOfWork.GetRepositories<Member>().Update(member);

            if (!string.IsNullOrWhiteSpace(member.HealthId.ToString()) || member.HealthId > 0)
            {
                var health = await _unitOfWork.GetRepositories<HealthRecord>().FirstOrDefaultAsync(h => h.Id == member.HealthId, ct);
                if (health is not null)
                {
                    health.Note = model.HealthNote;
                    _unitOfWork.GetRepositories<HealthRecord>().Update(health);
                }
            }

            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0;
        }
    }
}
