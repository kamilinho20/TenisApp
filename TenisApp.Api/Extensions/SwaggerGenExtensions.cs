using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TenisApp.Api.Extensions
{
    public static class SwaggerGenExtensions
    {
        public static void AddJwtBearerSecurityDefinition(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Specify auth token",
                Name = "Bearer",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            options.OperationFilter<OperationFilter>();
        }
    }

    public class OperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true ||
                context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (!isAuthorized) return;
 
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
 
        
            var bearerScheme = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" },
            };
 
            operation.Security.Add(new OpenApiSecurityRequirement()
            {
                [bearerScheme] = new string[] { }
            });
        }
    }
}
