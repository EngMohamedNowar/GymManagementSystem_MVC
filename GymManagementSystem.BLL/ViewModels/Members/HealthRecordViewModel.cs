using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class HealthRecordViewModel
    {
        [Range(0.1,300,ErrorMessage= "Height Must Be Greater Than") ]
        public decimal Height { get; set; }

        [Range(0.1, 300, ErrorMessage = "Weight Must Be Greater Than")]
        public decimal Weight { get; set; }

        [Required( ErrorMessage = "Blood Type Is Required")]
        [StringLength(3,ErrorMessage ="Blood Type Must be 3 Characters or less")]
        public string BloodType { get; set; } = default;

        public string? Note { get; set; } = default;
    }
}
