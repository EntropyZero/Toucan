using System.ComponentModel.DataAnnotations;

namespace ToucanSample.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
