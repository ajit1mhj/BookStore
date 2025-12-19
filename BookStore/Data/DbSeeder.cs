
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BookStore.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetRequiredService<UserManager<IdentityUser>>();
            var roleMgr = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles only if they do not exist
            if (!await roleMgr.RoleExistsAsync(Roles.Admin.ToString()))
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            }

            if (!await roleMgr.RoleExistsAsync(Roles.User.ToString()))
            {
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }

            // Create Admin user
            var adminEmail = "admin@gmail.com";
            var admin = await userMgr.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userMgr.CreateAsync(admin, "Admin@123");
            }

            // Assign Admin Role
            if (!await userMgr.IsInRoleAsync(admin, Roles.Admin.ToString()))
            {
                await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
}
