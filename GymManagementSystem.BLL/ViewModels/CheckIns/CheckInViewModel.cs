using System;

namespace GymManagementSystem.BLL.ViewModels.CheckIns;

public class CheckInViewModel
{
    public int Id { get; set; }
    public string MemberName { get; set; } = "—";
    public int MemberId { get; set; }
    public DateTime CheckInTime { get; set; }
    public string? Note { get; set; }
}
