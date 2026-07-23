using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Trainer : GymUser
    {
        public Specialization Spectiality { get; set; }
        public ICollection<Session> Sessions { get; set; }

    }
}
