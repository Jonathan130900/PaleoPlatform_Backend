using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventoPartecipazioniController : ControllerBase 
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public EventoPartecipazioniController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Partecipa(EventoPartecipazioneCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if the user has already joined the event
        var alreadyJoined = await _context.EventoPartecipazioni
            .AnyAsync(p => p.EventoId == dto.EventoId && p.UtenteId == userId);

        if (alreadyJoined)
            return BadRequest("Hai già partecipato a questo evento.");

        // Fetch the Evento to check available spots
        var evento = await _context.Eventi
            .Include(e => e.Partecipazioni)  // Include the participants to count them
            .FirstOrDefaultAsync(e => e.Id == dto.EventoId);

        if (evento == null)
            return NotFound("Evento non trovato.");

        // Check if there are available spots
        if (evento.Partecipazioni.Count >= evento.PostiDisponibili)
            return BadRequest("Non ci sono più posti disponibili per questo evento.");

        // Add the participation
        var partecipazione = new EventoPartecipazione
        {
            EventoId = dto.EventoId,
            UtenteId = userId
        };

        _context.EventoPartecipazioni.Add(partecipazione);
        await _context.SaveChangesAsync();

        return Ok("Partecipazione registrata.");
    }
}
