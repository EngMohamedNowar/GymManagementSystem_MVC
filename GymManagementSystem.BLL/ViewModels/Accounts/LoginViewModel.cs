using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModel.Accounts
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is Required !!")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Required !!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}