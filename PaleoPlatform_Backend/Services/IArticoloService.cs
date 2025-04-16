using PaleoPlatform_Backend.Models;

namespace PaleoPlatform_Backend.Services
{
    public interface IArticoloService
    {
        Task<IEnumerable<Articolo>> GetAllAsync();
        Task<Articolo> GetByIdAsync(int id);
        Task<Articolo> CreateAsync(Articolo articolo, IFormFile copertinaFile);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(Articolo articolo);

        Task<string> HandleFileUploadAsync(IFormFile copertinaFile, Articolo articolo);
        Task<string> SaveInlineImageAsync(IFormFile file);
    }
}