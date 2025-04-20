using Microsoft.AspNetCore.Identity;

namespace PaleoPlatform_Backend.Models
{
    public enum UserStatus
    {
        Active,
        Suspended,
        Banned
    }

    public class ApplicationUser : IdentityUser
    {
        public UserStatus Status { get; set; } = UserStatus.Active;
    }
}
