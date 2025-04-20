using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscussioniController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscussioniController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get all discussions or filter by topic
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? topicId)
        {
            var query = _context.Discussione
                .Include(d => d.Autore)
                .Include(d => d.Topic)
                .AsQueryable();

            if (topicId.HasValue)
                query = query.Where(d => d.TopicId == topicId.Value);

            var result = await query
                .Select(d => new
                {
                    d.Id,
                    d.Titolo,
                    d.Contenuto,
                    d.DataCreazione,
                    d.Upvotes,
                    d.Downvotes,
                    Autore = d.Autore.UserName,
                    Topic = d.Topic.Nome
                }).ToListAsync();

            return Ok(result);
        }

        // Get a single discussion
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var discussion = await _context.Discussione
                .Include(d => d.Autore)
                .Include(d => d.Topic)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (discussion == null)
                return NotFound("Discussione non trovata");

            return Ok(new
            {
                discussion.Id,
                discussion.Titolo,
                discussion.Contenuto,
                discussion.DataCreazione,
                discussion.Upvotes,
                discussion.Downvotes,
                Autore = discussion.Autore.UserName,
                Topic = discussion.Topic.Nome
            });
        }

        // Create new discussion (only if user is not suspended)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] DiscussioneCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || user.Status == UserStatus.Suspended || user.Status == UserStatus.Banned)
                return Forbid("Utente sospeso o bannato");

            var topic = await _context.Topics.FindAsync(dto.TopicId);
            if (topic == null)
                return BadRequest("Topic non valido");

            var discussion = new Discussione
            {
                Titolo = dto.Titolo,
                Contenuto = dto.Contenuto,
                AutoreId = userId,
                TopicId = dto.TopicId
            };

            _context.Discussione.Add(discussion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = discussion.Id }, new
            {
                discussion.Id,
                discussion.Titolo,
                discussion.Contenuto,
                discussion.DataCreazione
            });
        }

        // Vote on a discussion
        [HttpPost("{id}/vota")]
        [Authorize]
        public async Task<IActionResult> Vote(int id, [FromBody] VoteDto dto)
        {
            var user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null || user.Status != UserStatus.Active)
                return Forbid("Utente non autorizzato");

            var discussion = await _context.Discussione.FindAsync(id);
            if (discussion == null)
                return NotFound("Discussione non trovata");

            if (dto.IsUpvote)
                discussion.Upvotes++;
            else
                discussion.Downvotes++;

            await _context.SaveChangesAsync();

            return Ok(new { discussion.Upvotes, discussion.Downvotes });
        }
    }

}
