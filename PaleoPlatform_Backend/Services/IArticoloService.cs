using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;

namespace PaleoPlatform_Backend.Services
{
    public interface IArticoloService
    {
        Task<IEnumerable<Articolo>> GetAllAsync();
        Task<Articolo?> GetByIdAsync(int id);
        Task<Articolo> CreateAsync(Articolo articolo, IFormFile copertina);
        Task<string> HandleFileUploadAsync(IFormFile file, Articolo? articolo = null);
        Task<string> SaveInlineImageAsync(IFormFile file);
        Task<bool> UpdateAsync(Articolo articolo);
        Task<bool> DeleteAsync(int id);
    }
}