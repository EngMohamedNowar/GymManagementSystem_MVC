using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.ViewModels.BodyMeasurements;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IBodyMeasurementService
    {
        Task<Result> AddAsync(CreateBodyMeasurementViewModel model, CancellationToken ct = default);
        Task<IEnumerable<BodyMeasurementViewModel>> GetByMemberAsync(int memberId, CancellationToken ct = default);
    }
}
