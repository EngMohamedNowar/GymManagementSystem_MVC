using GymManagement.DbContexts;
using GymManagement.Models;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class MemberRepositories : GenericRepositories<Member>, IMemberRepositories 
    {

        public MemberRepositories(GymDbContext Context) :base(Context)
        {
        }


    }
}
