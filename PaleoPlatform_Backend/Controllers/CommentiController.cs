using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PaleoPlatform_Backend.Data;

namespace PaleoPlatform_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create comment endpoint
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment(CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user ID from the JWT

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId
            };

            _context.Commenti.Add(comment);
            await _context.SaveChangesAsync();

            // Return the created comment as a DTO
            var createdComment = new CommentoReadDto
            {
                Id = comment.Id,
                Contenuto = comment.Contenuto,
                UserName = (await _userManager.FindByIdAsync(userId)).UserName,
                CreatedAt = comment.DataPubblicazione,
                ParentCommentId = comment.ParentCommentId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, createdComment);
        }

        // Get a comment by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _context.Commenti
                .Where(c => c.Id == id)
                .Include(c => c.Utente) // Include user data for UserName
                .FirstOrDefaultAsync();

            if (comment == null)
                return NotFound("Commento non trovato");

            var commentDto = new CommentoReadDto
            {
                Id = comment.Id,
                Contenuto = comment.Contenuto,
                UserName = comment.Utente.UserName,
                CreatedAt = comment.DataPubblicazione,
                ParentCommentId = comment.ParentCommentId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes
            };

            return Ok(commentDto);
        }

        // Vote on a comment (upvote or downvote)
        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> VoteOnComment(VoteDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user ID from the JWT
            var comment = await _context.Commenti.FindAsync(dto.CommentoId);

            if (comment == null)
                return NotFound("Commento non trovato");

            // Upvote logic
            if (dto.IsUpvote)
            {
                comment.Upvotes++;
            }
            else
            {
                comment.Downvotes++;
            }

            await _context.SaveChangesAsync();

            // Return updated vote counts
            return Ok(new
            {
                comment.Upvotes,
                comment.Downvotes
            });
        }

        // Get comments (paginated if needed, modify as per your use case)
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _context.Commenti
                .Include(c => c.Utente) // Include the user to fetch their name
                .Select(c => new CommentoReadDto
                {
                    Id = c.Id,
                    Contenuto = c.Contenuto,
                    UserName = c.Utente.UserName,
                    CreatedAt = c.DataPubblicazione,
                    ParentCommentId = c.ParentCommentId,
                    Upvotes = c.Upvotes,
                    Downvotes = c.Downvotes
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost("articolo/{articoloId}")]
        [Authorize]
        public async Task<IActionResult> CreateCommentForArticolo(int articoloId, CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var articolo = await _context.Articoli.FindAsync(articoloId);
            if (articolo == null)
                return NotFound("Articolo non trovato");

            // Ensure only one of the two IDs is provided (either ArticoloId or DiscussioneId)
            if (dto.DiscussioneId.HasValue)
                return BadRequest("You cannot specify a DiscussioneId when commenting on an Articolo");

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId,
                ArticoloId = articoloId,
            };

            _context.Commenti.Add(comment);
            await _context.SaveChangesAsync();

            var createdDto = new CommentoReadDto
            {
                Id = comment.Id,
                Contenuto = comment.Contenuto,
                UserName = (await _userManager.FindByIdAsync(userId)).UserName,
                CreatedAt = comment.DataPubblicazione,
                ParentCommentId = comment.ParentCommentId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, createdDto);
        }

        [HttpPost("discussione/{discussioneId}")]
        [Authorize]
        public async Task<IActionResult> CreateCommentForDiscussione(int discussioneId, CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var discussione = await _context.Discussione.FindAsync(discussioneId);
            if (discussione == null)
                return NotFound("Discussione non trovata");

            // Ensure only one of the two IDs is provided (either ArticoloId or DiscussioneId)
            if (dto.ArticoloId.HasValue)
                return BadRequest("You cannot specify an ArticoloId when commenting on a Discussione");

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId,
                DiscussioneId = discussioneId,
            };

            _context.Commenti.Add(comment);
            await _context.SaveChangesAsync();

            var createdDto = new CommentoReadDto
            {
                Id = comment.Id,
                Contenuto = comment.Contenuto,
                UserName = (await _userManager.FindByIdAsync(userId)).UserName,
                CreatedAt = comment.DataPubblicazione,
                ParentCommentId = comment.ParentCommentId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes
            };

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, createdDto);
        }

        [HttpGet("articolo/{articoloId}")]
        public async Task<IActionResult> GetCommentsForArticolo(int articoloId)
        {
            var comments = await _context.Commenti
                .Where(c => c.ArticoloId == articoloId)
                .Include(c => c.Utente)
                .Select(c => new CommentoReadDto
                {
                    Id = c.Id,
                    Contenuto = c.Contenuto,
                    UserName = c.Utente.UserName,
                    CreatedAt = c.DataPubblicazione,
                    ParentCommentId = c.ParentCommentId,
                    Upvotes = c.Upvotes,
                    Downvotes = c.Downvotes
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpGet("discussione/{discussioneId}")]
        public async Task<IActionResult> GetCommentsForDiscussione(int discussioneId)
        {
            var comments = await _context.Commenti
                .Where(c => c.DiscussioneId == discussioneId)
                .Include(c => c.Utente)
                .Select(c => new CommentoReadDto
                {
                    Id = c.Id,
                    Contenuto = c.Contenuto,
                    UserName = c.Utente.UserName,
                    CreatedAt = c.DataPubblicazione,
                    ParentCommentId = c.ParentCommentId,
                    Upvotes = c.Upvotes,
                    Downvotes = c.Downvotes
                })
                .ToListAsync();

            return Ok(comments);
        }
    }
}