using System.ComponentModel.DataAnnotations;

namespace TenisApp.Shared.ViewModel
{
    public class LoginViewModel
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}