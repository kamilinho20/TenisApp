using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TenisApp.Core.Enum;
using TenisApp.Core.Model;

namespace TenisApp.Shared.ViewModel
{
    public class UserViewModel
    {
        [Required]
        public AppUser User { get; set; }
        public List<string> Roles { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}