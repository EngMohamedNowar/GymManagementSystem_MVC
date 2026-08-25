using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL
{
    public interface IUnitOfWork
    {
        IGenericRepositories<TEntity> GetRepositories<TEntity>() where TEntity : Base, new();
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        ISessionRepository sessionRepository { get; }
        IMembershipRepository membershipRepository { get; }
        IBookingRepository bookingRepository { get; }
    }
}
