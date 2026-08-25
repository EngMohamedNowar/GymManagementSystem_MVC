using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Trainers
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default;
        public string Email { get; set; } = default;
        public string Phone { get; set; } = default;
        public string Gender { get; set; } = default;
        // Member Details
        public string DateOfBirth { get; set; } = default;
        public string Address { get; set; } = default;
        public Specialization Specialization { get; set; }
    }
}
