using AutoMapper;
using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Sessions;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be greater than start date");
            if (model.StartDate < DateTime.Now) return Result.Validation("start date must be in the future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("capacity must beetween 1 and 25");  
            var trainer =await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound($"Trainer with id {model.TrainerId} not found");
            var category = await _unitOfWork.GetRepositories<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category is null) return Result.NotFound($"Category with id {model.CategoryId} not found");
            var isValid = Enum.TryParse<Specialization>(category.Name, out var categorySpeciality);
            if (!isValid || trainer.Spectiality != categorySpeciality) return Result.Validation("Can not create session with diffrent speciality");

            var session = _mapper.Map<Session>(model);
            _unitOfWork.GetRepositories<Session>().Add(session);
            var count =await _unitOfWork.SaveChanegesAsync(ct);
            return count > 0 ? Result.Ok() : Result.NotFound("Failed to create session");
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync(CancellationToken ct)
        {
            var categories = await _unitOfWork.GetRepositories<Category>().GetAllAsync(ct: ct);
            if (categories is null) return null;
            var categoriesDTOs = categories.Select(m => new CategorySelectViewModel()
            {
                Id = m.Id,
                Name = m.Name
            });
            return categoriesDTOs;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct)
        {
           var sessions = await _unitOfWork.sessionRepository.GetAllSessionWithTrainerAndCategoryAsync(ct);
            if (sessions is null || !sessions.Any()) return null;
            //var sessionsDTOs = sessions.Select(s => new SessionViewModel()
            // {
            //     Id = s.Id,
            //     CategoryName = s.Category.Name,
            //     Capacity = s.Capacity,
            //     Description = s.Description,
            //     StartDate = s.StartDate,
            //     EndDate = s.EndDate,
            //     TrainerName =s.Trainer.Name
            // });
            var sessionsDTOs = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in sessionsDTOs)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(session.Id,ct);
            }
            return sessionsDTOs;
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepositories<Trainer>().GetAllAsync(ct: ct);
            if (trainers is null) return null;
            var trainersDTOs = trainers.Select(m => new TrainerSelectViewModel()
            {
                Id = m.Id,
                Name = m.Name
            });
            return trainersDTOs;
        }

        public async Task<Result<SessionViewModel>> GetSessionDetailsById(int sessionId, CancellationToken ct)
        {
            var session = await _unitOfWork.sessionRepository.GetSessionByIdAsync(sessionId);
            if (session is null) return Result<SessionViewModel>.NotFound($"this session not found {sessionId}");
            var sessionDTOs = _mapper.Map<SessionViewModel>(session);
            sessionDTOs.AvailableSlots = session.Capacity - await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            return Result<SessionViewModel>.Ok(sessionDTOs);
        }

        public async Task<IEnumerable<SessionViewModel>> GetSessionsScheduleAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.sessionRepository.GetAllSessionWithTrainerAndCategoryAsync(ct);
            if (sessions is null || !sessions.Any()) return null;

            var sessionsDTOs = _mapper.Map<IEnumerable<SessionViewModel>>(sessions)
                .OrderBy(s => s.StartDate)
                .ToList();

            foreach (var session in sessionsDTOs)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return sessionsDTOs;
        }

        public async Task<Result<IEnumerable<AttendeeViewModel>>> GetSessionAttendeesAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.sessionRepository.GetSessionByIdAsync(sessionId, ct);
            if (session is null) return Result<IEnumerable<AttendeeViewModel>>.NotFound($"Session with id {sessionId} not found");

            var attendees = await _unitOfWork.sessionRepository.GetAttendeesBySessionIdAsync(sessionId, ct);
            var attendeesDTOs = attendees.Select(b => new AttendeeViewModel
            {
                MemberId = b.MemberId,
                MemberName = b.Member.Name,
                Phone = b.Member.Phone,
                IsAttended = b.IsAttended
            }).ToList();

            return Result<IEnumerable<AttendeeViewModel>>.Ok(attendeesDTOs);
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session =await _unitOfWork.GetRepositories<Session>().GetByIdAsync(sessionId, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound();
            if (session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Started");
            var bookingCount = await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0) return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Booking");


            var sessionDTOs = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(sessionDTOs);
        }

        public async Task<Result> UpdateSessionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct)
        {
            var session = await _unitOfWork.GetRepositories<Session>().GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound();
            if (session.StartDate <= DateTime.Now) return Result.Fail("Can Not Update Session That Has Already Started");
            var bookingCount = await _unitOfWork.sessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0) return Result.Fail("Can Not Update Session That Has Already Booking");
            if (model.StartDate >= model.EndDate) return Result.Fail("start date must be greater than end date");
            if (model.StartDate > DateTime.Now) return Result.Fail("start date must be in future");

            var trainer = await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer is null) return Result.NotFound($"Trainer with id {model.TrainerId} not found");
            var category = await _unitOfWork.GetRepositories<Category>().GetByIdAsync(session.CategoryId, ct);
            if (category is null) return Result.NotFound($"Category with id {session.CategoryId} not found");
            var isValid = Enum.TryParse<Specialization>(category.Name, out var categorySpeciality);
            if (!isValid || trainer.Spectiality != categorySpeciality) return Result.Validation("Can not create session with diffrent speciality");


            session.StartDate = model.StartDate;
            session.EndDate = model.EndDate;
            session.Description = model.Description;
            session.TrainerId = model.TrainerId;
            session.UpdatedAt = DateTime.Now;
            var count = await _unitOfWork.SaveChanegesAsync(ct);
            return count > 0 ? Result.Ok() : Result.Fail("Fail To Update");
        }

    }
}
