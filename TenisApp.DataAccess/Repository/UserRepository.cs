using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TenisApp.Core.Enum;
using TenisApp.DataAccess.Authentication;
using TenisApp.DataAccess.DbContext;

namespace TenisApp.DataAccess.Repository
{

    public interface IUserRepository
    {
        Task<Dictionary<string, List<string>>> GetUsersRoles();
        Task<Dictionary<string, List<Permission>>> GetUsersPermissions();
        Task<List<User>> GetUsers();
        Task UpdateUserPermissions(User user, List<Permission> permissions);
        Task UpdateUserRoles(User user, List<string> roles);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly UserPermissionComparer _permissionComparer;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _permissionComparer = new UserPermissionComparer();
        }
        public async Task<Dictionary<string, List<string>>> GetUsersRoles()
        {
            var userRoles = await _dbContext.UserRoles.AsNoTracking()
                .Join(_dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new {User = ur.UserId, Role = r}).ToListAsync();
            return userRoles.GroupBy(ur => ur.User).ToDictionary(g => g.Key, g => g.Select(r => r.Role.Name).ToList());
        }

        public async Task<Dictionary<string, List<Permission>>> GetUsersPermissions()
        {
            var userClaims = await _dbContext.UserClaims.AsNoTracking().Where(uc => uc.ClaimType == "Permission")
                .ToListAsync();
            return userClaims.GroupBy(uc => uc.UserId).ToDictionary(g => g.Key,
                g => g.Select(c => Enum.Parse<Permission>(c.ClaimValue)).ToList());
        }

        public Task<List<User>> GetUsers()
        {
            return _dbContext.Users.AsNoTracking().Include(u => u.AppUser).ToListAsync();
        }

        public async Task UpdateUserPermissions(User user, List<Permission> permissions)
        {
            var fromDb = await _dbContext.UserClaims.AsNoTracking()
                .Where(c => c.ClaimType == "Permission" && c.UserId == user.Id).ToListAsync();
            var fromModel = permissions.Select(p => new IdentityUserClaim<string>()
            {
                ClaimValue = p.ToString("G"),
                ClaimType = "Permission",
                UserId = user.Id
            }).ToList();
            var toDelete = fromDb.Except(fromModel, _permissionComparer);
            var toAdd = fromModel.Except(fromDb, _permissionComparer);
            _dbContext.RemoveRange(toDelete);
            await _dbContext.AddRangeAsync(toAdd);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserRoles(User user, List<string> roles)
        {
            var fromDb = await _dbContext.UserRoles.AsNoTracking().Where(ur => ur.UserId == user.Id).ToListAsync();
            var fromModel = (await _dbContext.Roles.AsNoTracking().Where(r => roles.Contains(r.Name)).ToListAsync()).Select(r => new IdentityUserRole<string>()
            {
                RoleId = r.Id,
                UserId = user.Id
            }).ToList();
            var toDelete = fromDb.Except(fromModel);
            var toAdd = fromModel.Except(fromDb);
            _dbContext.RemoveRange(toDelete);
            await _dbContext.AddRangeAsync(toAdd);
            await _dbContext.SaveChangesAsync();
        }

        public class UserPermissionComparer : IEqualityComparer<IdentityUserClaim<string>>
        {
            public bool Equals(IdentityUserClaim<string> x, IdentityUserClaim<string> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.UserId == y.UserId && x.ClaimType == y.ClaimType && x.ClaimValue == y.ClaimValue;
            }

            public int GetHashCode(IdentityUserClaim<string> obj)
            {
                return HashCode.Combine(obj.UserId, obj.ClaimType, obj.ClaimValue);
            }
        }
    }
}