using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.Notifications
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public string Type { get; set; } = default!;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
