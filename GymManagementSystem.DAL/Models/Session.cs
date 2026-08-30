using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Session : Base
    {
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Trainer Trainer { get; set; } = null!;
        public int TrainerId { get; set; }
        public Category Category { get; set; } = null!;
        public int CategoryId { get; set; }
        public ICollection<Booking> Members { get; set; } = new List<Booking>();
    }
}
