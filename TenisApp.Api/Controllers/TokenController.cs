using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Authentication;
using TenisApp.Shared.ViewModel;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace TenisApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;

        public TokenController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null) return NotFound();
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return ProcessFailResult(result);
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            claims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Tokens:Issuer"], _config["Tokens:Audience"],
                claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);
            var tokenData = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };

            return Created("", tokenData);
        }

        private IActionResult ProcessFailResult(SignInResult result)
        {
            if (result == SignInResult.Failed)
            {
                return BadRequest("Wrong password!");
            }
            if (result == SignInResult.NotAllowed)
            {
                return BadRequest("Account not confirmed!");
            }
            if (result == SignInResult.LockedOut)
            {
                return BadRequest("Account is locked!");
            }

            return Problem("Unknown problem!", statusCode: 500);
        }
    }
}
