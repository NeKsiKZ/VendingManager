using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VendingManager.Models;

namespace VendingManager.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            string[] roleNames = { "Admin", "Operator" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@vending.com";
            string adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    Console.WriteLine("Nie udało się stworzyć użytkownika admina: " + createResult.Errors.FirstOrDefault()?.Description);
                }
            }
        }
    }
}