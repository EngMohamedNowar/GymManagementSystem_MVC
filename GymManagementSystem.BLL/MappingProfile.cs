using AutoMapper;
using GymManagementSystem.BLL.ViewModes.Members;
using GymManagementSystem.BLL.ViewModes.Sessions;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateMemberDTOs, Member>()
                .ForMember(D=>D.Address,O=>O.MapFrom(s=>new Address()
                {
                    BuildingNumber = s.BuildingNumber,
                    City = s.City,
                    Street = s.Street
                })).ForMember(D=>D.Health,O=>O.MapFrom(s=>new HealthRecord
                {
                    BloodType =s.HealthRecordViewModel.BloodType,
                    Height  =s.HealthRecordViewModel.Height,
                    Note = s.HealthRecordViewModel.Note,
                    weight = s.HealthRecordViewModel.Weight
                }));
            CreateMap<Member,MemberViewModel>()
                .ForMember(D=> D.DateOfBirth, O=> O.MapFrom(S=>S.DateOfBirth.ToString()))
                .ForMember(D=>D.Address,O=>O.MapFrom(s=>$"{s.Address.BuildingNumber } - {s.Address.Street} - {s.Address.City}"));
            CreateMap<HealthRecord,HealthRecordViewModel>();
            CreateMap<Member, UpdateMemberDTOs>();
            CreateMap<Session, UpdateSessionViewModel>();
            CreateMap<Session, SessionViewModel>()
                .ForMember(D => D.TrainerName, opt => opt.MapFrom(s => s.Trainer.Name))
                .ForMember(D => D.CategoryName, opt => opt.MapFrom(s => s.Category.Name));
            CreateMap<CreateSessionViewModel, Session>();
        }
    }
}
