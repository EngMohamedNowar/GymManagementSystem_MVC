using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Members
{
    public class MemberProfileEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone")]
        public string Phone { get; set; } = default!;

        [Required(ErrorMessage = "Building number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Building number must be greater than 0")]
        [Display(Name = "Building Number")]
        public int BuildingNumber { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters")]
        [Display(Name = "City")]
        public string City { get; set; } = default!;

        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Street must be between 2 and 100 characters")]
        [Display(Name = "Street")]
        public string Street { get; set; } = default!;

        [Display(Name = "Health Note")]
        public string? HealthNote { get; set; }
    }
}
