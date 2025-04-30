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

            // Get or create 'deleted_user'
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

                // Lock the deleted_user account permanently
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

            // Reassign Discussions
            var discussions = await _context.Discussione
                .Where(d => d.AutoreId == userToDelete.Id)
                .ToListAsync();

            foreach (var discussion in discussions)
                discussion.AutoreId = deletedUser.Id;

            // Clean up other stuff (e.g., event tickets)
            var biglietti = await _context.Biglietti
              .Where(b => b.UtenteId == userToDelete.Id)
              .ToListAsync();
               foreach (var b in biglietti)
               b.UtenteId = deletedUser.Id;

            await _context.SaveChangesAsync(); 

            // Delete user
            var deleteResult = await _userManager.DeleteAsync(userToDelete);
            return deleteResult.Succeeded;
        }

        public async Task<bool> ReactivateUserAsync(string userId, string requestingUserId)
        {
            var userToReactivate = await _userManager.FindByIdAsync(userId);
            if (userToReactivate == null)
                return false;

            // Protect admin/moderator from reactivating themselves
            if (userToReactivate.Id == requestingUserId)
                return false;

            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            if (requestingUser == null)
                return false;

            var isRequesterAdminOrMod = await _userManager.IsInRoleAsync(requestingUser, "Amministratore") ||
                                         await _userManager.IsInRoleAsync(requestingUser, "Moderatore");
            if (!isRequesterAdminOrMod)
                return false;

            // Ensure the user is not in a banned state
            if (userToReactivate.Status != UserStatus.Banned)
                return false;

            // Set status to active
            userToReactivate.Status = UserStatus.Active;
            var updateResult = await _userManager.UpdateAsync(userToReactivate);

            if (!updateResult.Succeeded)
                return false;

            // Unlock the user account
            await _userManager.SetLockoutEnabledAsync(userToReactivate, false);
            await _userManager.SetLockoutEndDateAsync(userToReactivate, null);

            return true;
        }

        public async Task<bool> SuspendUserAsync(string userId, string requestingUserId)
        {
            var userToSuspend = await _userManager.FindByIdAsync(userId);
            if (userToSuspend == null)
                return false;

            // Protect admin/moderator from suspending themselves
            if (userToSuspend.Id == requestingUserId)
                return false;

            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            if (requestingUser == null)
                return false;

            var isRequesterAdminOrMod = await _userManager.IsInRoleAsync(requestingUser, "Amministratore") ||
                                         await _userManager.IsInRoleAsync(requestingUser, "Moderatore");
            if (!isRequesterAdminOrMod)
                return false;

            // Ensure the user is not in a banned or system state
            if (userToSuspend.Status == UserStatus.Banned || userToSuspend.Status == UserStatus.System)
                return false;

            // Set status to suspended
            userToSuspend.Status = UserStatus.Suspended;
            var updateResult = await _userManager.UpdateAsync(userToSuspend);

            if (!updateResult.Succeeded)
                return false;

            // Lock the user account temporarily (define suspension period if needed)
            await _userManager.SetLockoutEnabledAsync(userToSuspend, true);
            await _userManager.SetLockoutEndDateAsync(userToSuspend, DateTimeOffset.UtcNow.AddMonths(1)); // Suspend for 1 month (example)

            return true;
        }
        public async Task<bool> BanUserAsync(string userId, string requestingUserId)
        {
            var userToBan = await _userManager.FindByIdAsync(userId);
            if (userToBan == null)
                return false;

            // Protect admin/moderator from banning themselves
            if (userToBan.Id == requestingUserId)
                return false;

            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            if (requestingUser == null)
                return false;

            var isRequesterAdminOrMod = await _userManager.IsInRoleAsync(requestingUser, "Amministratore") ||
                                         await _userManager.IsInRoleAsync(requestingUser, "Moderatore");
            if (!isRequesterAdminOrMod)
                return false;

            var roles = await _userManager.GetRolesAsync(userToBan);
            if (roles.Contains("System"))
                return false;

            // Set status to banned
            userToBan.Status = UserStatus.Banned;
            var updateResult = await _userManager.UpdateAsync(userToBan);

            if (!updateResult.Succeeded)
                return false;

            // Immediately lockout the user
            await _userManager.SetLockoutEnabledAsync(userToBan, true);
            await _userManager.SetLockoutEndDateAsync(userToBan, DateTimeOffset.MaxValue);

            return true;
        }
    }
}
