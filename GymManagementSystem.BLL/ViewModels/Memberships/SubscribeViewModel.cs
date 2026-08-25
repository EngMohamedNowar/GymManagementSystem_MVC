using System.ComponentModel.DataAnnotations;

namespace GymManagementSystem.BLL.ViewModels.Memberships
{
    public class SubscribeViewModel
    {
        [Required(ErrorMessage = "Please select a plan")]
        [Display(Name = "Plan")]
        public int PlanId { get; set; }

        [Display(Name = "Discount Code")]
        public string? DiscountCode { get; set; }
    }
}
