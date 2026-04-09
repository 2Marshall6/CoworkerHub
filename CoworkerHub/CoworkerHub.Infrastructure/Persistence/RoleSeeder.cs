using CoworkerHub.Domain.Entities;
using CoworkerHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CoworkerHub.Infrastructure.Persistens
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { AppRoles.Admin, AppRoles.Manager, AppRoles.User };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }

            if (await userManager.FindByEmailAsync("admin@hub.com") == null)
            {
                var admin = new User { UserName = "Admin", Email = "admin@hub.com" };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }
    }
}