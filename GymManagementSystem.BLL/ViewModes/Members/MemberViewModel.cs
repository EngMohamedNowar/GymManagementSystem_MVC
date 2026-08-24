using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModes.Members
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; } 
        public string Name { get; set; }   = default;
        public string Email { get; set; }  = default;
        public string Phone { get; set; }  = default;
        public string Gender { get; set; } = default;
        // Member Details
        public string DateOfBirth { get; set; }= default;
        public string Address { get; set; }    = default;
        public string? PlanName { get; set; } = default;
        public string? MemberShipStartDate { get; set; }= default;
        public string? MemberShipEndDate { get; set; } = default;
    }
}
