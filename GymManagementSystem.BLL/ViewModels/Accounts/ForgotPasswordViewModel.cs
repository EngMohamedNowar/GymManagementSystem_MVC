using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModel.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;
    }
}
