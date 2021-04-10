using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Authentication;

namespace TenisApp.DataAccess.DbContext
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Term> Terms { get; set; }
        public DbSet<Relief> Reliefs { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
