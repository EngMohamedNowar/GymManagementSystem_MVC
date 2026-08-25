using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.Audit
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Entity { get; set; } = default!;
        public string? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
