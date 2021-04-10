using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenisApp.Core.Enum;
using TenisApp.Core.Model;
using TenisApp.DataAccess.Authentication;
using TenisApp.DataAccess.DbContext;

namespace TenisApp.DataAccess.Repository
{
    public interface IRoleRepository
    {
        Task<Dictionary<string, List<Permission>>> GetRolePermissions();
        Task<Dictionary<string, List<AppUser>>> GetRoleUsers();
        Task UpdatePermissions(IdentityRole role, List<Permission> modelPermissions);
        Task UpdateUsers(IdentityRole role, List<AppUser> modelUsers);
    }

    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly RolePermissionComparer _rolePermissionComparer;

        public RoleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _rolePermissionComparer = new RolePermissionComparer();
        }

        public async Task<Dictionary<string, List<Permission>>> GetRolePermissions()
        {
            var roleClaims = await _dbContext.RoleClaims.AsNoTracking().Where(rc => rc.ClaimType == "Permission").ToListAsync();
            return roleClaims.GroupBy(rc => rc.RoleId).ToDictionary(g => g.Key, g => g.Select(c => Enum.Parse<Permission>(c.ClaimValue)).ToList());
        }

        public async Task<Dictionary<string, List<AppUser>>> GetRoleUsers()
        {
            var userRoles = await _dbContext.UserRoles.AsNoTracking()
                .Join(_dbContext.Users.Include(u => u.AppUser), ur => ur.UserId, u => u.Id, (ur, u) => new {Role = ur.RoleId, User = u}).ToListAsync();
            return userRoles.GroupBy(r => r.Role).ToDictionary(g => g.Key, g => g.Select(r => r.User.AppUser).ToList());
        }

        public async Task UpdatePermissions(IdentityRole role, List<Permission> modelPermissions)
        {
            var fromDb = await _dbContext.RoleClaims.AsNoTracking()
                .Where(rc => rc.ClaimType == "Permission" && rc.RoleId == role.Id).ToListAsync();
            var fromModel = modelPermissions.Select(p => new IdentityRoleClaim<string>()
            {
                ClaimValue = p.ToString("G"),
                ClaimType = "Permission",
                RoleId = role.Id
            }).ToList();
            var toDelete = fromDb.Except(fromModel, _rolePermissionComparer);
            var toAdd = fromModel.Except(fromDb, _rolePermissionComparer);
            _dbContext.RoleClaims.RemoveRange(toDelete);
            await _dbContext.RoleClaims.AddRangeAsync(toAdd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUsers(IdentityRole role, List<AppUser> modelUsers)
        {
            var fromDb = await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.RoleId == role.Id).ToListAsync();
            //TODO: być może zmienić z id na email
            var modelIds = modelUsers.Select(u => u.Id).ToList();
            var fromModel = (await _dbContext.Users.AsNoTracking().Where(u => modelIds.Contains(u.AppUserId))
                .ToListAsync()).Select(u => new IdentityUserRole<string>()
            {
                UserId = u.Id,
                RoleId = role.Id
            }).ToList();
            var toDelete = fromDb.Except(fromModel);
            var toAdd = fromModel.Except(fromDb);
            _dbContext.RemoveRange(toDelete);
            await _dbContext.AddRangeAsync(toAdd);
            await _dbContext.SaveChangesAsync();
        }

        public class RolePermissionComparer : IEqualityComparer<IdentityRoleClaim<string>>
        {
            public bool Equals(IdentityRoleClaim<string> x, IdentityRoleClaim<string> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.RoleId == y.RoleId && x.ClaimType == y.ClaimType && x.ClaimValue == y.ClaimValue;
            }

            public int GetHashCode(IdentityRoleClaim<string> obj)
            {
                return HashCode.Combine(obj.RoleId, obj.ClaimType, obj.ClaimValue);
            }
        }
    }
}