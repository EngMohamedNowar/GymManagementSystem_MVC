using System;
using System.Collections.Generic;

namespace GymManagementSystem.BLL.ViewModels.BodyMeasurements
{
    public class BodyMeasurementViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime Date { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal? BodyFat { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
