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
using System.Collections.Generic;

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

        // -------------------- Helper Method --------------------
        private CommentoReadDto MapToDtoWithReplies(Commento comment)
        {
            return new CommentoReadDto
            {
                Id = comment.Id,
                Contenuto = comment.Contenuto,
                UserName = comment.Utente?.UserName ?? "deleted_user",
                CreatedAt = comment.DataPubblicazione,
                ParentCommentId = comment.ParentCommentId,
                Upvotes = comment.Upvotes,
                Downvotes = comment.Downvotes,
                Replies = comment.Replies?
                    .OrderBy(r => r.DataPubblicazione)
                    .Select(r => MapToDtoWithReplies(r))
                    .ToList() ?? new List<CommentoReadDto>()
            };
        }

        // -------------------- Create Generic Comment --------------------
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComment(CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId
            };

            _context.Commenti.Add(comment);
            await _context.SaveChangesAsync();

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

        // -------------------- Get Comment by ID with Replies --------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _context.Commenti
                .Include(c => c.Utente)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Utente)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                return NotFound("Commento non trovato");

            return Ok(MapToDtoWithReplies(comment));
        }

        // -------------------- Vote on a Comment --------------------
        [HttpPost("vote")]
        [Authorize]
        public async Task<IActionResult> VoteOnComment(VoteDto dto)
        {
            var comment = await _context.Commenti.FindAsync(dto.CommentoId);
            if (comment == null)
                return NotFound("Commento non trovato");

            if (dto.IsUpvote)
                comment.Upvotes++;
            else
                comment.Downvotes++;

            await _context.SaveChangesAsync();

            return Ok(new { comment.Upvotes, comment.Downvotes });
        }

        // -------------------- Get All Top-Level Comments --------------------
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            var comments = await _context.Commenti
                .Where(c => c.ParentCommentId == null)
                .Include(c => c.Utente)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Utente)
                .ToListAsync();

            var result = comments.Select(c => MapToDtoWithReplies(c)).ToList();
            return Ok(result);
        }

        // -------------------- Comment on Articolo --------------------
        [HttpPost("articolo/{articoloId}")]
        [Authorize]
        public async Task<IActionResult> CreateCommentForArticolo(int articoloId, CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var articolo = await _context.Articoli.FindAsync(articoloId);
            if (articolo == null) return NotFound("Articolo non trovato");

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId,
                ArticoloId = articoloId
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

        // -------------------- Comment on Discussione --------------------
        [HttpPost("discussione/{discussioneId}")]
        [Authorize]
        public async Task<IActionResult> CreateCommentForDiscussione(int discussioneId, CommentoCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var discussione = await _context.Discussione.FindAsync(discussioneId);
            if (discussione == null) return NotFound("Discussione non trovata");

            var comment = new Commento
            {
                Contenuto = dto.Contenuto,
                UtenteId = userId,
                ParentCommentId = dto.ParentCommentId,
                DiscussioneId = discussioneId
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

        // -------------------- Get Comments for Articolo --------------------
        [HttpGet("articolo/{articoloId}")]
        public async Task<IActionResult> GetCommentsForArticolo(int articoloId)
        {
            var articolo = await _context.Articoli
                .Include(a => a.Autore)
                .FirstOrDefaultAsync(a => a.Id == articoloId);

            if (articolo == null) return NotFound();

            var topLevelComments = await _context.Commenti
                .Where(c => c.ArticoloId == articoloId && c.ParentCommentId == null)
                .Include(c => c.Utente)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Utente)
                .OrderByDescending(c => c.DataPubblicazione)
                .Select(c => MapToDtoWithReplies(c))
                .ToListAsync();

            return Ok(new ArticoloReadDto
            {
                Id = articolo.Id,
                Titolo = articolo.Titolo,
                Contenuto = articolo.Contenuto,
                AutoreUserName = articolo.Autore.UserName,
                DataPubblicazione = articolo.DataPubblicazione,
                CopertinaUrl = articolo.CopertinaUrl,
                Commenti = topLevelComments
            });
        }

        // -------------------- Get Comments for Discussione --------------------
        [HttpGet("discussione/{discussioneId}")]
        public async Task<IActionResult> GetCommentsForDiscussione(int discussioneId)
        {
            var comments = await _context.Commenti
                .Where(c => c.DiscussioneId == discussioneId && c.ParentCommentId == null)
                .Include(c => c.Utente)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.Utente)
                .ToListAsync();

            var result = comments.Select(c => MapToDtoWithReplies(c)).ToList();
            return Ok(result);
        }

        // -------------------- Delete Comment --------------------
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _context.Commenti
                .Where(c => c.Id == id && c.UtenteId == userId)
                .Include(c => c.Replies)
                .FirstOrDefaultAsync();

            if (comment == null)
                return NotFound("Commento non trovato o non appartiene all'utente");

            _context.Commenti.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}