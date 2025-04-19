using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;

namespace PaleoPlatform_Backend.Services
{
    public class UtenteService : IUtenteService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UtenteService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> DeleteUserAsync(string userId, string requestingUserId)
        {
            var userToDelete = await _userManager.FindByIdAsync(userId);
            if (userToDelete == null)
                return false;

            // Protect admin from deleting themselves
            if (userToDelete.Id == requestingUserId)
                return false;

            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            if (requestingUser == null)
                return false;

            var isRequesterAdmin = await _userManager.IsInRoleAsync(requestingUser, "Amministratore");
            if (!isRequesterAdmin)
                return false;

            // Get or create '[deleted]' user
            var deletedUser = await _userManager.FindByNameAsync("deleted_user");
            if (deletedUser == null)
            {
                deletedUser = new ApplicationUser
                {
                    UserName = "deleted_user",
                    Email = "deleted_user@deleted.com",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(deletedUser, "FakePassword123!");
                if (!result.Succeeded)
                    return false;

                // Lock the [deleted] user account permanently
                await _userManager.SetLockoutEnabledAsync(deletedUser, true);
                await _userManager.SetLockoutEndDateAsync(deletedUser, DateTimeOffset.MaxValue);
            }
            else
            {
                // Ensure lockout is enforced even if the user already existed
                await _userManager.SetLockoutEnabledAsync(deletedUser, true);
                await _userManager.SetLockoutEndDateAsync(deletedUser, DateTimeOffset.MaxValue);
            }


            // Reassign comments
            var comments = await _context.Commenti
                .Where(c => c.UtenteId == userToDelete.Id)
                .ToListAsync();

            foreach (var comment in comments)
                comment.UtenteId = deletedUser.Id;

            await _context.SaveChangesAsync();

            // Delete user
            var deleteResult = await _userManager.DeleteAsync(userToDelete);
            return deleteResult.Succeeded;
        }
    }
}
