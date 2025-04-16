using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Services;

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

        public ArticoliController(IArticoloService service, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _service = service;
            _mapper = mapper;
            _userManager = userManager;
            _environment = environment;
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

            if (dto.Copertina != null)
            {
                articolo.CopertinaUrl = await _service.HandleFileUploadAsync(dto.Copertina, articolo);
            }

            articolo.Titolo = dto.Titolo;
            articolo.Contenuto = dto.Contenuto;

            await _service.UpdateAsync(articolo);
            return NoContent();
        }

        // DELETE api/articoli/{id} (Requires Admin or Moderator)
        [Authorize(Roles = "Amministratore,Moderatore")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var articolo = await _service.GetByIdAsync(id);
            if (articolo == null) return NotFound();

            if (!string.IsNullOrEmpty(articolo.CopertinaUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, articolo.CopertinaUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : StatusCode(500, "Failed to delete article");
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