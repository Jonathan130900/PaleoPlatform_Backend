using Microsoft.AspNetCore.Identity;

namespace PaleoPlatform_Backend.Models
{
    public enum UserStatus
    {
        Active,
        Suspended,
        Banned,
        System
    }

    public class ApplicationUser : IdentityUser
    {
        public UserStatus Status { get; set; } = UserStatus.Active;
        public DateTime? SuspendedUntil { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public ICollection<Biglietto> Biglietti { get; set; }
    }
}
 