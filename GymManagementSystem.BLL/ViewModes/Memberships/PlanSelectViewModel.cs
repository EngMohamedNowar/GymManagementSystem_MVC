using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModes.Memberships
{
    public class PlanSelectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
    }
}
