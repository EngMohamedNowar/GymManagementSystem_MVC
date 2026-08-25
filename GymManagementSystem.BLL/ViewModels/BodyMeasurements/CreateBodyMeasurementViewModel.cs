using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.BodyMeasurements
{
    public class CreateBodyMeasurementViewModel
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Weight is required")]
        [Range(1, 500, ErrorMessage = "Weight must be between 1 and 500")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required")]
        [Range(0.1, 3, ErrorMessage = "Height must be between 0.1 and 3 (m)")]
        [Display(Name = "Height (m)")]
        public decimal Height { get; set; }

        [Range(0, 100, ErrorMessage = "Body fat must be between 0 and 100")]
        [Display(Name = "Body Fat %")]
        public decimal? BodyFat { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
