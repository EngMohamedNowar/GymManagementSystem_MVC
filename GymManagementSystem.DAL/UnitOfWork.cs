using GymManagement.DbContexts;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<string, object> _repositories = [];
        private readonly GymDbContext _context;
        private readonly ISessionRepository _sessionRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IBookingRepository _bookingRepository;


        public UnitOfWork(GymDbContext context, ISessionRepository sessionRepository, IMembershipRepository membershipRepository, IBookingRepository bookingRepository) 
        {
            _context = context;
            _sessionRepository = sessionRepository;
            _membershipRepository = membershipRepository;
            _bookingRepository = bookingRepository;
        }

        public ISessionRepository sessionRepository => _sessionRepository;
        public IMembershipRepository membershipRepository => _membershipRepository;
        public IBookingRepository bookingRepository => _bookingRepository;

        public IGenericRepositories<TEntity> GetRepositories<TEntity>() where TEntity : Base, new()
        {

            var typeName = typeof(TEntity).Name;
            if(_repositories.TryGetValue(typeName,out object? value)){
                return value as IGenericRepositories<TEntity>;
            }
            var repo = new GenericRepositories<TEntity>(_context);
            _repositories.Add(typeName, repo);
            return repo;
        }

        public async Task<int> SaveChanegesAsync(CancellationToken ct = default)
         =>   await _context.SaveChangesAsync(ct);

    } 
}
