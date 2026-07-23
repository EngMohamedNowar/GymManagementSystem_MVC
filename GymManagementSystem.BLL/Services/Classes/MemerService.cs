using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Members;
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
        private readonly IGenericRepositories<Member> _memberRepository;
        private readonly IGenericRepositories<MemberShip> _memberShip;
        private readonly IGenericRepositories<HealthRecord> _memberHealthRecord;
        private readonly IGenericRepositories<Booking> _bookingRepository;

        public MemberService(IGenericRepositories<Member> memberRepository
            ,IGenericRepositories<MemberShip> memberShip
            , IGenericRepositories<HealthRecord> memberHealthRecord,
            IGenericRepositories<Booking> bookingRepository)
        {
            _memberRepository = memberRepository;
            _memberShip = memberShip;
            _memberHealthRecord = memberHealthRecord;
            _bookingRepository = bookingRepository;

        }
        public async Task<bool> CreateMemberAsync(CreateMemberDTOs model, CancellationToken ct = default)
        {
            var emailExist = await _memberRepository.AnyAsync(m => m.Email == model.Email,ct);
            var phoneExist = await _memberRepository.AnyAsync(m => m.Phone == model.Phone,ct);
            if (emailExist || phoneExist) return false;
            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
                Health = new HealthRecord()
                {
                    Height = model.HealthRecordViewModel.Height,
                    BloodType = model.HealthRecordViewModel.BloodType,
                    weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note
                }
                
                
            };
          var count = await _memberRepository.AddAsync(member);
            return count > 0;
        }

        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null) return false;
            var hasFutureSession = await _bookingRepository.AnyAsync(M => M.MemberId == memberId && M.Session.StartDate > DateTime.Now, ct);
            if (hasFutureSession) return false;
            var count = await _memberRepository.DeleteAsync(member,ct);
            return count > 0;
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepository.GetAllAsync(ct: ct);
            if (members is null) return null;

            // Mapping --> Casting 
            var membersDTOs = members.Select(member => new MemberViewModel()
            {
                Id = member.Id,
                Photo = member.Photo,
                Name = member.Name,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                Email = member.Email
            });
            return membersDTOs;
        }

        public async Task<MemberViewModel?> GetMemberDetalisAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberId, ct); // Member Entity
            if (member is null) return null;
            // Mapping --> Casting 
            var membersDTOs = new MemberViewModel()
            {
                Photo = member.Photo,
                Name = member.Name,
                Phone = member.Phone,
                Gender = member.Gender.ToString(),
                Email = member.Email,
                DateOfBirth = member.DateOfBirth.ToString(),
                Address = $"Building Number: {member.Address.BuildingNumber} ,Street:{member.Address.Street}, City:{member.Address.City}"
            };
            var activePlans = await _memberShip.FirstOrDefaultAsync(
              m => m.Id == member.Id && m.EndDate > DateTime.Now, ct);

            if(activePlans is not null)
            {
                membersDTOs.PlanName = activePlans.Plan.Name;
                membersDTOs.MemberShipEndDate = activePlans.EndDate.ToString();
                membersDTOs.MemberShipStartDate = activePlans.CreatedAt.ToString();    
            }
            return membersDTOs;
        }

        public async Task<HealthRecordViewModel?> GetMemberHelthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var memberHealthRecord = await _memberHealthRecord.FirstOrDefaultAsync(m => m.Id == memberId,ct);
            if (memberHealthRecord is null) return null;
            var healthRecord = new HealthRecordViewModel()
            {
                Height = memberHealthRecord.Height,
                Note = memberHealthRecord.Note,
                BloodType = memberHealthRecord.BloodType,
                Weight = memberHealthRecord.weight
            };
            return healthRecord;
        }

        public async Task<UpdateMemberDTOs> MemberToUpdateAsync(int meberId, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(meberId, ct);
            if (member is null) return null;
            var memberUpdated = new UpdateMemberDTOs()
            {
                Photo = member.Photo,
                Name = member.Name,
                Email =member.Email,
                Phone =member.Phone,
                DateOfBirth = member.DateOfBirth,
                BuildingNumber = member.Address.BuildingNumber,
                City = member.Address.City,
                Street = member.Address.Street
            };
            return memberUpdated;
        }

        public async Task<bool> UpdateMemberAsync(int memberId, UpdateMemberDTOs model, CancellationToken ct = default)
        {
            var member = await _memberRepository.GetByIdAsync(memberId,ct);
            if (member == null) return false;

            var emailExist = await _memberRepository.AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct);
            var phoneExist = await _memberRepository.AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);

            if (emailExist || phoneExist) return false;

            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            var count = await _memberRepository.UpdateAsync(member);
            return count > 0;
        }
    }
}
