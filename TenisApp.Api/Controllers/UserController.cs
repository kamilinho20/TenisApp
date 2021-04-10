using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TenisApp.Core.Enum;
using TenisApp.DataAccess.Authentication;
using TenisApp.DataAccess.Repository;
using TenisApp.Shared.ViewModel;

namespace TenisApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(Role.SuperAdmin))]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _repository;
        private readonly UserManager<User> _userManager;

        public UserController(ILogger<UserController> logger, IUserRepository repository, UserManager<User> userManager)
        {
            _logger = logger;
            _repository = repository;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _repository.GetUsers();
                var permissions = await _repository.GetUsersPermissions();
                var roles = await _repository.GetUsersRoles();
                return Ok(users.Select(u => new UserViewModel()
                {
                    User = u.AppUser,
                    Permissions = permissions.ContainsKey(u.Id) ? permissions[u.Id] : new List<Permission>(),
                    Roles = roles.ContainsKey(u.Id) ? roles[u.Id] : new List<string>()
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.User.Email);
                if (user == null) return NotFound();
                await _repository.UpdateUserRoles(user, model.Roles);
                await _repository.UpdateUserPermissions(user, model.Permissions);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(UserViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.User.Email);
                if (user == null) return NotFound();
                await _userManager.DeleteAsync(user);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }
    }
}
