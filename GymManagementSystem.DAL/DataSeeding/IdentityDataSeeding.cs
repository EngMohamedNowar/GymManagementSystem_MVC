using GymManagementSystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            IConfiguration configuration,
            CancellationToken ct = default
        )
        {
            try
            {
                // Secrets are sourced from configuration (e.g. appsettings.json / environment variables)
                // under the "AdminSeed" section. Provide safe placeholder defaults so the seeder still
                // works locally without committing real credentials.
                var adminPassword = configuration["AdminSeed:Password"] ?? "sdfg@HJKL123";
                var superAdminEmail = configuration["AdminSeed:SuperAdminEmail"] ?? "superadmin@fitgym.com";
                var adminEmail = configuration["AdminSeed:AdminEmail"] ?? "mohamednowar2002@gmail.com";
                var superAdminPhone = configuration["AdminSeed:SuperAdminPhone"] ?? "01557722675";
                var adminPhone = configuration["AdminSeed:AdminPhone"] ?? "01000000002";

                var hasUsers = await userManager.Users.AnyAsync(ct);
                var hasRoles = await roleManager.Roles.AnyAsync(ct);

                if (hasUsers && hasRoles) return;

                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin"),
                    new IdentityRole("Member"),
                };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name!))
                        {
                            await roleManager.CreateAsync(role);
                        }
                    }
                }
                else
                {
                    // Ensure all required roles exist even if other roles/users are already seeded.
                    var requiredRoles = new[] { "SuperAdmin", "Admin", "Member" };
                    foreach (var roleName in requiredRoles)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
                }

                if (!hasUsers)
                {
                    var superAdmin = new ApplicationUser()
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = superAdminEmail,
                        Email = superAdminEmail,
                        PhoneNumber = superAdminPhone
                    };

                    await userManager.CreateAsync(superAdmin, adminPassword);
                    await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");

                    var admin = new ApplicationUser()
                    {
                        FirstName = "App",
                        LastName = "Admin",
                        UserName = adminEmail,
                        Email = adminEmail,
                        PhoneNumber = adminPhone
                    };

                    await userManager.CreateAsync(admin, adminPassword);
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
