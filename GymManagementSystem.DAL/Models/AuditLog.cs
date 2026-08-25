using System;
using System.Collections.Generic;

namespace GymManagementSystem.DAL.Models
{
    public class AuditLog : Base
    {
        public string UserName { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Entity { get; set; } = default!;
        public string? EntityId { get; set; }
        public string? Details { get; set; }
    }
}
