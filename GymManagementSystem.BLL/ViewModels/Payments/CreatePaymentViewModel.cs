using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Payments
{
    public class CreatePaymentViewModel
    {
        public int MembershipId { get; set; }

        [Display(Name = "Membership")]
        public string MembershipLabel { get; set; } = default!;

        [Display(Name = "Discount Code")]
        public string? DiscountCode { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Method")]
        public string Method { get; set; } = "Cash";

        [Display(Name = "Reference")]
        public string? Reference { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
