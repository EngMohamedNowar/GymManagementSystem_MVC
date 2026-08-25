using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Models
{
    public class Notification : Base
    {
        public string? UserId { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Type { get; set; } = "System";
        public bool IsRead { get; set; }
    }
}
