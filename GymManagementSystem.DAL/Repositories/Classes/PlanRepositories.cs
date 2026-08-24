using GymManagement.DbContexts;
using GymManagement.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class PlanRepositories : GenericRepositories<Plan>, IPlanRepositories 
    {

        public PlanRepositories(GymDbContext Context) :base(Context)
        {
        }
    }
}
