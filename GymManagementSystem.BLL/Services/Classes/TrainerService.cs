using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.Trainers;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.Enums;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepositories<Trainer>().GetAllAsync(ct: ct);

            // Mapping
            var trainersDTOs = trainers.Select(trainer => new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender.ToString(),
                Specialization = trainer.Spectiality
                //Address = $"Building Number: {trainer.Address.BuildingNumber} ,Street:{trainer.Address.Street}, City:{trainer.Address.City}",
            });
            return trainersDTOs;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerDTOs model, CancellationToken ct = default)
        {
            var phoneExist = await _unitOfWork.GetRepositories<Trainer>().AnyAsync(m => m.Phone == model.Phone, ct);
            var emailExist = await _unitOfWork.GetRepositories<Trainer>().AnyAsync(m => m.Email == model.Email, ct);
            if (emailExist || phoneExist) return false;

            var trainerDTOs = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                Spectiality = model.Specialization,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                }
            };
            _unitOfWork.GetRepositories<Trainer>().Add(trainerDTOs);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int memberId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(memberId, ct);
            if (trainer is null) return null;

            var trainerDTOs = new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Specialization = trainer.Spectiality,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Gender = trainer.Gender.ToString(),
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"Building Number: {trainer.Address.BuildingNumber} ,Street:{trainer.Address.Street}, City:{trainer.Address.City}"
            };

            return trainerDTOs;
        }

        public async Task<UpdateTrainerDTOs?> TrainerToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(memberId, ct);
            if (trainer is null) return null;

            var trainerDTOs = new UpdateTrainerDTOs()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                BuildingNumber = trainer.Address.BuildingNumber,
                City = trainer.Address.City,
                Phone = trainer.Phone,
                Street = trainer.Address.Street,
                Specialization = trainer.Spectiality
            };
            return trainerDTOs;
        }

        public async Task<bool> UpdateTrainerAsync(int memberId, UpdateTrainerDTOs model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(memberId, ct);
            if (trainer is null) return false;

            var emailExist = await _unitOfWork.GetRepositories<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct);
            var phoneExist = await _unitOfWork.GetRepositories<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);
            if (emailExist || phoneExist) return false;

            // ?? ??? model (????? ??????? ?????? ?? ??????) ??? trainer (??? Entity ???????)
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Spectiality = model.Specialization;

            _unitOfWork.GetRepositories<Trainer>().Update(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int memberId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepositories<Trainer>().GetByIdAsync(memberId, ct);
            if (trainer is null) return false;

            var hasFutureSessions = await _unitOfWork.GetRepositories<Session>().AnyAsync(s => s.TrainerId == memberId && s.EndDate > DateTime.UtcNow, ct);
            if (hasFutureSessions)
                return false;

            _unitOfWork.GetRepositories<Trainer>().Delete(trainer);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0;
        }
    }
}