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
using TenisApp.Api.Authorization;
using TenisApp.Core.Enum;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Authentication;
using TenisApp.DataAccess.Repository;
using TenisApp.Shared.ViewModel;

namespace TenisApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = nameof(Role.SuperAdmin))]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleController> _logger;

        public RoleController(RoleManager<IdentityRole> roleManager, IRoleRepository roleRepository, ILogger<RoleController> logger)
        {
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var rolePermissions = await _roleRepository.GetRolePermissions();
                var users = await _roleRepository.GetRoleUsers();
                return Ok(roles.Select(r => new RoleViewModel()
                {
                    Role = r.Name,
                    Users = users.ContainsKey(r.Id) ? users[r.Id] : new List<AppUser>(),
                    Permissions = rolePermissions.ContainsKey(r.Id) ? rolePermissions[r.Id] : new List<Permission>()
                }));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            try
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
                if (role == null) return NotFound();
                await _roleRepository.UpdatePermissions(role, model.Permissions);
                await _roleRepository.UpdateUsers(role, model.Users);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            try
            {
                var role = new IdentityRole(model.Role);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded) return BadRequest(result.Errors);
                await _roleRepository.UpdatePermissions(role, model.Permissions);
                await _roleRepository.UpdateUsers(role, model.Users);
                return Created("", model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem("Database error occured!", statusCode: 500);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(RoleViewModel model)
        {
            try
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == model.Role);
                if (role == null) return NotFound();
                await _roleManager.DeleteAsync(role);
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
