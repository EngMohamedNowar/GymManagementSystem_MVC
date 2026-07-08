using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Category : Base
    {
        public string Name { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
