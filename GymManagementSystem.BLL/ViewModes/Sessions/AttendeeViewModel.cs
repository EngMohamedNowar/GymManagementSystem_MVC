using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.ViewModes.Sessions
{
    public class AttendeeViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public bool IsAttended { get; set; }
    }
}
