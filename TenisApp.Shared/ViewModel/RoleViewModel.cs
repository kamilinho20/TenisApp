using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TenisApp.Core.Enum;
using TenisApp.Core.Model;

namespace TenisApp.Shared.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        public string Role { get; set; }
        public List<AppUser> Users { get; set; }
        public List<Permission> Permissions { get; set; }
    }
}