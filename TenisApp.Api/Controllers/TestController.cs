using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TenisApp.Api.Authorization;
using TenisApp.Core.Enum;

namespace TenisApp.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = nameof(Permission.TestIndex))]
        public IEnumerable<int> Index()
        {
            return Enumerable.Range(1,10);
        }

        [HttpGet("strings")]
        [Authorize(Policy = nameof(Permission.TestStrings))]
        public IEnumerable<string> Strings()
        {
            return Enumerable.Range(1, 10).Select(i => Guid.NewGuid().ToString());
        }

        [HttpGet("claims")]
        public IEnumerable<object> GetClaims()
        {
            return User.Claims.Select(c => new
            {
                c.Type,
                c.Value
            });
        }
    }
}
