using GymManagementSystem.BLL.Common;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.BLL.ViewModels.BodyMeasurements;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class BodyMeasurementService : IBodyMeasurementService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BodyMeasurementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> AddAsync(CreateBodyMeasurementViewModel model, CancellationToken ct = default)
        {
            if (model is null) return Result.Validation("Model is required");

            var member = await _unitOfWork.GetRepositories<Member>().GetByIdAsync(model.MemberId, ct);
            if (member is null) return Result.NotFound($"Member with id {model.MemberId} not found");

            var measurement = new BodyMeasurement
            {
                MemberId = model.MemberId,
                Date = model.Date,
                Weight = model.Weight,
                Height = model.Height,
                BodyFat = model.BodyFat,
                Notes = model.Notes
            };

            _unitOfWork.GetRepositories<BodyMeasurement>().Add(measurement);
            var count = await _unitOfWork.SaveChangesAsync(ct);
            return count > 0 ? Result.Ok() : Result.Fail("Failed to add measurement");
        }

        public async Task<IEnumerable<BodyMeasurementViewModel>> GetByMemberAsync(int memberId, CancellationToken ct = default)
        {
            var all = await _unitOfWork.GetRepositories<BodyMeasurement>().GetAllAsync(tracking: false, ct: ct);
            return all
                .Where(m => m.MemberId == memberId)
                .OrderBy(m => m.Date)
                .Select(m => new BodyMeasurementViewModel
                {
                    Id = m.Id,
                    MemberId = m.MemberId,
                    Date = m.Date,
                    Weight = m.Weight,
                    Height = m.Height,
                    BodyFat = m.BodyFat,
                    Notes = m.Notes,
                    CreatedAt = m.CreatedAt
                })
                .ToList();
        }
    }
}
