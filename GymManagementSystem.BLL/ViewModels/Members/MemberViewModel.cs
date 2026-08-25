using GymManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; } 
        public string Name { get; set; }   = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string Phone { get; set; }  = string.Empty;
        public string Gender { get; set; } = string.Empty;
        // Member Details
        public string DateOfBirth { get; set; }= string.Empty;
        public string Address { get; set; }    = string.Empty;
        public string? PlanName { get; set; } = default;
        public string? MemberShipStartDate { get; set; }= default;
        public string? MemberShipEndDate { get; set; } = default;
    }
}
