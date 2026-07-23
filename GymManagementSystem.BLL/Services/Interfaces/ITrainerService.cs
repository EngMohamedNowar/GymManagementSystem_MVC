using GymManagementSystem.BLL.ViewModes.Trainers;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);

        Task<TrainerViewModel?> GetTrainerDetalisAsync(int memberId, CancellationToken ct = default);

        Task<bool> CreateTrainerAsync(CreateTrainerDTOs model, CancellationToken ct = default);

        Task<UpdateTrainerDTOs> TrainerToUpdateAsync(int meberId, CancellationToken ct = default);

        Task<bool> UpdateTrainerAsync(int memberId, UpdateTrainerDTOs model, CancellationToken ct = default);

        Task<bool> DeleteTrainerAsync(int memberId, CancellationToken ct = default);
    }
}
