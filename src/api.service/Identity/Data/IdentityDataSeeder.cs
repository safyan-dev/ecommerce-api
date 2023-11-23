using api.building.Persistence;
using api.service.Shared.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;

namespace api.service.Identity.Data
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityDataSeeder(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAllAsync()
        {
            await SeedRoles();
            await SeedUsers();
        }

        public int Order => 1;

        private async Task SeedRoles()
        {
            if (!await _roleManager.RoleExistsAsync(ApplicationRole.Admin.Name))
                await _roleManager.CreateAsync(ApplicationRole.Admin);

            if (!await _roleManager.RoleExistsAsync(ApplicationRole.User.Name))
                await _roleManager.CreateAsync(ApplicationRole.User);
        }

        private async Task SeedUsers()
        {
            if (await _userManager.FindByEmailAsync("mehdi@test.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "safyan",
                    FirstName = "Safyan",
                    LastName = "Yaqoob",
                    Email = "safyan@gmail.com",
                };

                var result = await _userManager.CreateAsync(user, "123456");

                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, ApplicationRole.Admin.Name);
            }
        }
    }
}
