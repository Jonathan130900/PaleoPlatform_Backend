namespace PaleoPlatform_Backend.Services
{
    public interface IUtenteService
    {
        Task<bool> DeleteUserAsync(string userId, string requestingUserId);
        Task<bool> BanUserAsync(string userId, string requestingUserId);
        Task<bool> ReactivateUserAsync(string userId, string requestingUserId);
        Task<bool> SuspendUserAsync(string userId, string requestingUserId);
    }
}
