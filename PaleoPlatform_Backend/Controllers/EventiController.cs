using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;
using AutoMapper;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventiController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 
        private readonly IMapper _mapper;

        public EventiController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get all events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoReadDto>>> GetAllEvents(int page = 1, int pageSize = 10)
        {
            var eventi = await _context.Eventi
                                       .Skip((page - 1) * pageSize)
                                       .Take(pageSize)
                                       .ToListAsync();
            return Ok(_mapper.Map<List<EventoReadDto>>(eventi));
        }

        // Get a specific event by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var evento = await _context.Eventi.Include(e => e.Biglietti).FirstOrDefaultAsync(e => e.Id == id);

            if (evento == null)
                return NotFound("Evento non trovato");

            return Ok(_mapper.Map<EventoReadDto>(evento));
        }

        // Create a new event
        [HttpPost]
        [Authorize(Roles = "Amministratore,Moderatore")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEvent([FromForm] EventoCreateDto eventoDto)
        {
            if (string.IsNullOrWhiteSpace(eventoDto.Nome))
            {
                return BadRequest("Event name is required.");
            }

            var evento = _mapper.Map<Evento>(eventoDto);

            if (eventoDto.Copertina != null)
            {
                var imageUrl = await SaveEventImageAsync(eventoDto.Copertina);
                if (imageUrl == null)
                {
                    return BadRequest("Invalid file or no file uploaded.");
                }
                evento.CopertinaUrl = imageUrl;
            }

            _context.Eventi.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = evento.Id }, _mapper.Map<EventoReadDto>(evento));
        }

        // Save event image (helper method)
        private async Task<string?> SaveEventImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return null;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return null;
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine("wwwroot", "eventImages", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/eventImages/{fileName}";
        }

        // Update an existing event
        [HttpPut("{id}")]
        [Authorize(Roles = "Amministratore,Moderatore")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] EventoUpdateDto eventoDto)
        {
            var evento = await _context.Eventi.FindAsync(id);
            if (evento == null)
            {
                return NotFound("Evento non trovato");
            }

            if (!string.IsNullOrWhiteSpace(eventoDto.Nome)) evento.Nome = eventoDto.Nome;
            if (!string.IsNullOrWhiteSpace(eventoDto.Descrizione)) evento.Descrizione = eventoDto.Descrizione;
            if (eventoDto.DataInizio.HasValue) evento.DataInizio = eventoDto.DataInizio.Value;
            if (eventoDto.DataFine.HasValue) evento.DataFine = eventoDto.DataFine.Value;
            if (!string.IsNullOrWhiteSpace(eventoDto.Luogo)) evento.Luogo = eventoDto.Luogo;
            if (eventoDto.Prezzo.HasValue) evento.Prezzo = eventoDto.Prezzo.Value;
            if (eventoDto.PostiDisponibili.HasValue) evento.PostiDisponibili = eventoDto.PostiDisponibili.Value;

            if (eventoDto.Copertina != null)
            {
                var imageUrl = await SaveEventImageAsync(eventoDto.Copertina);
                if (imageUrl == null)
                {
                    return BadRequest("Invalid file or no file uploaded.");
                }
                evento.CopertinaUrl = imageUrl;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Delete an event
        [HttpDelete("{id}")]
        [Authorize(Roles = "Amministratore,Moderatore")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var evento = await _context.Eventi.FindAsync(id);

            if (evento == null)
                return NotFound("Evento non trovato");

            _context.Eventi.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
