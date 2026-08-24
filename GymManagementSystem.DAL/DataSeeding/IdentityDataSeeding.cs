using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger logger,
            CancellationToken ct = default
        )
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync();
                var hasRoles = await roleManager.Roles.AnyAsync();

                if (hasUsers && hasRoles) return;

                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin"),
                };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name))
                        {
                            await roleManager.CreateAsync(role);
                        }
                    }
                }

                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Nowar",
                        UserName = "mohamednowar_",
                        Email = "mohamednowar2002@gmail.com",
                        PhoneNumber = "01557722675"
                    };

                    await userManager.CreateAsync(superAdmin, "sdfg@HJKL123");
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                    var admin = new ApplicationUser()
                    {
                        FirstName = "ahmed",
                        LastName = "hamed",
                        UserName = "ahmedhamed",
                        Email = "ahmedhamed@gmail.com",
                        PhoneNumber = "01001680106"
                    };

                    await userManager.CreateAsync(admin, "sdfg@HJKL123");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            catch (Exception ex)
            {                  
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
