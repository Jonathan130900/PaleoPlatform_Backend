﻿using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Web;

namespace PaleoPlatform_Backend.Services
{
    public class ArticoloService : IArticoloService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ArticoloService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<Articolo> CreateAsync(Articolo articolo, IFormFile copertinaFile)
        {
            var autore = await _context.Users.FindAsync(articolo.AutoreId);
            if (autore == null)
                throw new ArgumentException("Autore not found");

            _context.Articoli.Add(articolo);
            await _context.SaveChangesAsync();

            // Sanitize title for folder name
            var safeTitle = HttpUtility.UrlEncode(articolo.Titolo);
            var articleFolder = Path.Combine(_environment.WebRootPath, "uploads", safeTitle);

            Directory.CreateDirectory(articleFolder); // No need to check exists first

            if (copertinaFile != null && copertinaFile.Length > 0)
            {
                var thumbnailPath = Path.Combine(articleFolder, "thumbnail");
                Directory.CreateDirectory(thumbnailPath);

                var thumbnailFileName = Guid.NewGuid().ToString() + Path.GetExtension(copertinaFile.FileName);
                var thumbnailFilePath = Path.Combine(thumbnailPath, thumbnailFileName);

                await SaveFileAsync(copertinaFile, thumbnailFilePath);

                // URL encode the title in the path
                articolo.CopertinaUrl = $"/uploads/{safeTitle}/thumbnail/{thumbnailFileName}";

                _context.Articoli.Update(articolo);
                await _context.SaveChangesAsync();

                // Verify file was saved correctly
                if (!File.Exists(thumbnailFilePath))
                {
                    throw new Exception($"File failed to save to {thumbnailFilePath}");
                }
            }

            return articolo;
        }

        private async Task SaveFileAsync(IFormFile file, string destinationPath)
        {
            try
            {
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                using (var stream = new FileStream(destinationPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Verify file was written
                if (new FileInfo(destinationPath).Length != file.Length)
                {
                    throw new Exception("File size mismatch after save");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error saving file to {destinationPath}", ex);
            }
        }

        public async Task<string> HandleFileUploadAsync(IFormFile copertinaFile, Articolo articolo)
        {
            var articleFolder = Path.Combine(_environment.WebRootPath, "uploads", $"{articolo.Titolo}");
            if (!Directory.Exists(articleFolder))
                Directory.CreateDirectory(articleFolder);

            var thumbnailPath = Path.Combine(articleFolder, "thumbnail");
            if (!Directory.Exists(thumbnailPath))
                Directory.CreateDirectory(thumbnailPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(copertinaFile.FileName);
            var filePath = Path.Combine(thumbnailPath, fileName);

            await SaveFileAsync(copertinaFile, filePath);

            return $"/uploads/{articolo.Titolo}/thumbnail/{fileName}";
        }

        public async Task<string> SaveInlineImageAsync(IFormFile file)
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var articolo = await _context.Articoli.FindAsync(id);
            if (articolo == null) return false;

            if (!string.IsNullOrEmpty(articolo.CopertinaUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, articolo.CopertinaUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Articoli.Remove(articolo);
            await _context.SaveChangesAsync();
            return true;
        }

        // Modify GetAllAsync to eagerly load comments and user data
        public async Task<IEnumerable<Articolo>> GetAllAsync()
        {
            return await _context.Articoli
                .Include(a => a.Commenti) // Eagerly load comments
                .ThenInclude(c => c.Utente) // User info for comments
                .Include(a => a.Autore) // Load article's author as well
                .ToListAsync();
        }

        public async Task<Articolo?> GetByIdAsync(int id)
        {
            return await _context.Articoli
                .Include(a => a.Autore)
                .Include(a => a.Commenti)  // Eagerly load comments
                    .ThenInclude(c => c.Utente) // Include the user info for each comment
                                                //.ThenInclude(c => c.Risposte)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> UpdateAsync(Articolo articolo)
        {
            _context.Articoli.Update(articolo);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
