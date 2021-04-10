using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using TenisApp.Core.Model;

namespace TenisApp.DataAccess.Authentication
{
    public class User : IdentityUser
    {
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}