using GymManagementSystem.BLL.ViewModels.Trainers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);

        Task<TrainerViewModel?> GetTrainerDetailsAsync(int memberId, CancellationToken ct = default);

        Task<bool> CreateTrainerAsync(CreateTrainerDTOs model, CancellationToken ct = default);

        Task<UpdateTrainerDTOs> TrainerToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<bool> UpdateTrainerAsync(int memberId, UpdateTrainerDTOs model, CancellationToken ct = default);

        Task<bool> DeleteTrainerAsync(int memberId, CancellationToken ct = default);
    }
}
