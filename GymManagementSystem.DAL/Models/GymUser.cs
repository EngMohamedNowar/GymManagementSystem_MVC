using GymManagementSystem.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public abstract class GymUser : Base
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Address Address { get; set; } = null!;
    }
    public class Address
    {
        public int BuildingNumber { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

    }
}
