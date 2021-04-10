using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TenisApp.DataAccess.Authentication;
using TenisApp.DataAccess.DbContext;
using Microsoft.IdentityModel.Tokens;
using TenisApp.Api.Authorization;
using TenisApp.Api.Extensions;
using TenisApp.Api.Service;
using TenisApp.Core.Enum;
using TenisApp.Core.Interface;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Repository;
using TenisApp.Localization;

namespace TenisApp.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(Configuration["ConnectionStrings:MainDatabase"]));
            services.AddIdentity<User,IdentityRole>(opts => opts.SignIn.RequireConfirmedEmail = true).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.AddJwtBearerAuthentication(Configuration.GetSection("Tokens").Get<TokenConfig>());
            services.AddTransient<IEmailSender, EmailSender>(s =>
                new EmailSender(Configuration.GetSection("Mailer").Get<MailerConfig>()));
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(opts =>
            {
                foreach (var permission in typeof(Permission).GetEnumValues().Cast<Permission>())
                {
                    opts.AddPolicy(permission.ToString(), policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TenisApp.Api", Version = "v1" });
                c.AddJwtBearerSecurityDefinition();
            });
            services.AddLocalization(opts => opts.ResourcesPath = "Resources");
            services.AddTransient<ILocalizer, SharedResource>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICourtRepository, CourtRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TenisApp.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
