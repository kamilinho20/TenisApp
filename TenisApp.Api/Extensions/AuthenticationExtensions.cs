using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace TenisApp.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static void AddJwtBearerAuthentication(this IServiceCollection services, TokenConfig config)
        {
            services.AddAuthentication(opts => opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters() {
                        ValidIssuer = config.Issuer,
                        ValidAudience = config.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key))
                    };
                });
        }
    }

    public class TokenConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
    }
}
