using api.service.Shared.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api_example.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<IReadOnlyList<ApplicationUser>> FindAllUserWithRoleAsync(this UserManager<ApplicationUser> userManager)
        {
            return await userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        }
        public static async Task<ApplicationUser?> FindUserWithRoleByIdAsync(this UserManager<ApplicationUser> userManager, Guid userId)
        {
            return await userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(r => r.RefreshTokens)
                .Include(a => a.AccessTokens)
                .FirstOrDefaultAsync(e => e.Id == userId);
        }
        public static async Task<ApplicationUser?> FindUserWithRoleByUserNameAsync(this UserManager<ApplicationUser> userManager,string userName)
        {
            return await userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(x => x.AccessTokens)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public static async Task<ApplicationUser?> FindUserWithRoleByEmailAsync(this UserManager<ApplicationUser> userManager,string email)
        {
            return await userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
