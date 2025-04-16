using Microsoft.AspNetCore.Identity;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace PaleoPlatform_Backend.Helpers
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Utente", "Moderatore", "Amministratore" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                    Console.WriteLine($"Created role '{role}': {result.Succeeded}");
                }
            }

            string adminEmail = "admin@paleo.com";
            string adminPassword = "Admin@123";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(admin, adminPassword);
                Console.WriteLine($"Admin creation result: {createResult.Succeeded}");

                if (createResult.Succeeded)
                {
                    var addToRole = await userManager.AddToRoleAsync(admin, "Amministratore");
                    Console.WriteLine($"Assigned admin role: {addToRole.Succeeded}");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                        Console.WriteLine($"Error: {error.Description}");
                }
            }
            else
            {
                Console.WriteLine("Admin already exists.");
            }
        }
    }

}
