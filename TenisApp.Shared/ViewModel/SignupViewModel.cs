using System.ComponentModel.DataAnnotations;

namespace TenisApp.Shared.ViewModel
{
    public class SignupViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}