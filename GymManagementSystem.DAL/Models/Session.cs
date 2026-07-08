using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Models
{
    public class Session : Base
    {
        public string Description { get; set; }
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Trainer Trainer { get; set; }
        public int TrainerId { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Booking> Members { get; set; }


    }
}
