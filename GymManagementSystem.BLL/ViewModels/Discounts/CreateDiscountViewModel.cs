using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Discounts
{
    public class CreateDiscountViewModel
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 50 characters")]
        [Display(Name = "Code")]
        public string Code { get; set; } = default!;

        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Type")]
        public string Type { get; set; } = "Percent"; // "Percent" | "Fixed"

        [Required(ErrorMessage = "Value is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Value must be positive")]
        [Display(Name = "Value")]
        public decimal Value { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }
    }
}
