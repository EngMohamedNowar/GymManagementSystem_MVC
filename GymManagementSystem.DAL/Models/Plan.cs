using GymManagementSystem.DAL.Models;

namespace GymManagement.Models
{
    public class Plan : Base
    {
        public string Name { get; set; } = default;
        public string Description { get; set; } = default;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool IsActive { get; set; }
        public ICollection<MemberShip> Members { get; set; }
    }
}
