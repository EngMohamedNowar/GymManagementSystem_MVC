using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Plans
{
    public class PlanViewModel
    {
        public int  Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
