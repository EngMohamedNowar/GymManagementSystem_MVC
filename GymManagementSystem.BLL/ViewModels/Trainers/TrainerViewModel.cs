using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Trainers
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        // Member Details
        public string DateOfBirth { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Specialization Specialization { get; set; }
    }
}
