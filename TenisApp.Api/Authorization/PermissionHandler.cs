using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TenisApp.DataAccess.Authentication;

namespace TenisApp.Api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<User> _userManager;

        public PermissionHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var user = await _userManager.GetUserAsync(context.User);
            var claims = await _userManager.GetClaimsAsync(user);
            if (claims.Any(c => c.Type == "Permission" && c.Value == requirement.Permission.ToString()))
            {
                context.Succeed(requirement);
            }
        }
    }
}
