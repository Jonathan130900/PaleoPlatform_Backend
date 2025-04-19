using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Services;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Data;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticoliController : ControllerBase
    {
        private readonly IArticoloService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ApplicationDbContext _context;

        public ArticoliController(IArticoloService service, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment;
            _context = context;
        }

        // GET api/articoli
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticoloReadDto>>> GetAll()
        {
            var articoli = await _service.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ArticoloReadDto>>(articoli));
        }

        // GET api/articoli/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticoloReadDto>> GetById(int id)
        {
            var articolo = await _service.GetByIdAsync(id);
            if (articolo == null) return NotFound();
            return Ok(_mapper.Map<ArticoloReadDto>(articolo));
        }

        // POST api/articoli (Requires Admin or Moderator)
        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ArticoloCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("No user ID found in token.");
            }

            var articolo = _mapper.Map<Articolo>(dto);
            articolo.AutoreId = userId;
            articolo.DataPubblicazione = DateTime.UtcNow;

            if (dto.Copertina == null)
                return BadRequest("Copertina (thumbnail) is required.");

            try
            {
                var created = await _service.CreateAsync(articolo, dto.Copertina); // ✅ pass IFormFile correctly
                var readDto = _mapper.Map<ArticoloReadDto>(created);
                return Ok(readDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error creating article: " + ex.Message);
            }
        }

        // PUT api/articoli/{id} (Requires Admin or Moderator)
        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] ArticoloCreateDto dto)
        {
            var articolo = await _service.GetByIdAsync(id);
            if (articolo == null) return NotFound();

            // Handle file upload and update logic
            if (dto.Copertina != null)
            {
                var newCopertinaUrl = await _service.HandleFileUploadAsync(dto.Copertina, articolo);
                articolo.CopertinaUrl = newCopertinaUrl;
            }

            // Update article fields
            articolo.Titolo = dto.Titolo;
            articolo.Contenuto = dto.Contenuto;

            await _service.UpdateAsync(articolo);
            return NoContent();
        }

        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the article to be deleted
            var articolo = await _service.GetByIdAsync(id);
            if (articolo == null)
            {
                return NotFound();  // If article doesn't exist, return 404
            }

            // Retrieve the [deleted] user
            var deletedUser = await _userManager.FindByEmailAsync("deleted_user@deleted.com");
            if (deletedUser == null)
            {
                return StatusCode(500, "Failed to find the deleted_user user");
            }

            // Retrieve all comments for this article using the injected _context
            var comments = await _context.Commenti
                .Where(c => c.ArticoloId == id)
                .ToListAsync();

            // Reassign comments to the [deleted] user
            foreach (var comment in comments)
            {
                comment.UtenteId = deletedUser.Id;  // Assign to the [deleted] user
            }

            // Save changes to comments
            await _context.SaveChangesAsync();

            // Delete the article
            var success = await _service.DeleteAsync(id);
            if (success)
            {
                // Handle cover image deletion (if any)
                if (!string.IsNullOrEmpty(articolo.CopertinaUrl))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, articolo.CopertinaUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                return NoContent();  // Article and comments reassigned and deleted successfully
            }
            else
            {
                return StatusCode(500, "Failed to delete article");
            }
        }


        // POST api/articoli/upload-inline-image
        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadInlineImage([FromForm] InlineImageUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest("Nessun file inviato.");

            try
            {
                var fileName = await _service.SaveInlineImageAsync(dto.File);
                return Ok(new { fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Errore durante il caricamento dell'immagine: " + ex.Message);
            }
        }

    }
}
