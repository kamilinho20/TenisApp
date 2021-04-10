using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal.Account.Manage;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using TenisApp.Api.Authorization;
using TenisApp.Core.Enum;
using TenisApp.Core.Interface;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Authentication;
using TenisApp.Shared.ViewModel;

namespace TenisApp.Api.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILocalizer _localizer;

        public AccountController(UserManager<User> userManager, IEmailSender emailSender, ILocalizer localizer)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _localizer = localizer;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SignupViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var appUser = new AppUser()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var user = new User() {AppUser = appUser, Email = model.Email, UserName = model.Email};
            var isFirst = !await _userManager.Users.AnyAsync();
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(Confirm), "Account", new {token, email = user.Email},
                    Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Account confirmation link", confirmationLink);
            }
            catch (Exception e)
            {
                await _userManager.DeleteAsync(user);
                return Problem(e.Message, statusCode: 500);
            }
            
            if (isFirst)
            {
                result = await _userManager.AddToRoleAsync(user, nameof(Role.SuperAdmin));
            }
            return result.Succeeded ? Created("", appUser) : Problem("User not created!", statusCode:500);
        }

        [HttpPost]
        public async Task<IActionResult> ResetRequest(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action(nameof(Reset), "Account", new
                {
                    token, email = user.Email
                }, Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, _localizer["Password reset link"], resetLink);

            }
            catch (Exception e)
            {
                return Problem(e.Message, statusCode: 500);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Reset([Required] string token, [Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            return View(new ResetViewModel() {Email = email, Token = token});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset([Required][FromForm] ResetViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound();
            if (!ModelState.IsValid) return View(model);
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded) return View("ResetSuccess");
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Confirm([Required] string token, [Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound();
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? (IActionResult) View(user) : BadRequest(result.Errors);
        }
    }
}
