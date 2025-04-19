namespace PaleoPlatform_Backend.Services
{
    public interface IUtenteService
    {
        Task<bool> DeleteUserAsync(string userId, string requestingUserId);
    }
}
