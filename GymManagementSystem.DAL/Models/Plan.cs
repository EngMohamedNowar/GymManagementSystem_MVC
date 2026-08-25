using GymManagementSystem.DAL.Models;

namespace GymManagement.Models
{
    public class Plan : Base
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool IsActive { get; set; }
        public ICollection<MemberShip> Members { get; set; } = new List<MemberShip>();
    }
}
