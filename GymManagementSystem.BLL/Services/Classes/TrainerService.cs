using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModes.Trainers;
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
        private readonly IGenericRepositories<Trainer> _trainerRepository;
        private readonly IGenericRepositories<Session> _sessionRepository;

        public TrainerService(IGenericRepositories<Trainer> trainerRepository, IGenericRepositories<Session> sessionRepository)
        {
            _trainerRepository = trainerRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _trainerRepository.GetAllAsync(ct: ct);

            // Mapping
            var trainersDTOs = trainers.Select(trainer => new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                Specialization = trainer.Spectiality,
                //Address = $"Building Number: {trainer.Address.BuildingNumber} ,Street:{trainer.Address.Street}, City:{trainer.Address.City}",
            });
            return trainersDTOs;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerDTOs model, CancellationToken ct = default)
        {
            var emailExist = await _trainerRepository.AnyAsync(m => m.Email == model.Email, ct);
            var phoneExist = await _trainerRepository.AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExist || phoneExist) return false;

            var trainerDTOs = new Trainer()
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
                }
            };
            var count = await _trainerRepository.AddAsync(trainerDTOs);
            return count > 0;
        }

        public async Task<TrainerViewModel?> GetTrainerDetalisAsync(int memberId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(memberId, ct);
            if (trainer is null) return null;

            var trainerDTOs = new TrainerViewModel()
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Specialization = trainer.Spectiality,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToString(),
                Address = $"Building Number: {trainer.Address.BuildingNumber} ,Street:{trainer.Address.Street}, City:{trainer.Address.City}"
            };

            return trainerDTOs;
        }

        public async Task<UpdateTrainerDTOs> TrainerToUpdateAsync(int meberId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(meberId, ct);
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
            var trainer = await _trainerRepository.GetByIdAsync(memberId, ct);
            if (trainer is null) return false;

            var emailExist = await _trainerRepository.AnyAsync(m => m.Email == model.Email && m.Id != memberId, ct);
            var phoneExist = await _trainerRepository.AnyAsync(m => m.Phone == model.Phone && m.Id != memberId, ct);
            if (emailExist || phoneExist) return false;

            // من الـ model (القيم الجديدة الجاية من الفورم) إلى trainer (الـ Entity المحفوظ)
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Spectiality = model.Specialization;

            var count = await _trainerRepository.UpdateAsync(trainer);
            return count > 0;
        }

        public async Task<bool> DeleteTrainerAsync(int memberId, CancellationToken ct = default)
        {
            var trainer = await _trainerRepository.GetByIdAsync(memberId, ct);
            if (trainer is null) return false;

            var hasFutureSessions = await _sessionRepository.AnyAsync(s => s.TrainerId == memberId && s.EndDate > DateTime.UtcNow, ct);
            if (hasFutureSessions)
                return false;

            var count = await _trainerRepository.DeleteAsync(trainer);
            return count > 0;
        }
    }
}