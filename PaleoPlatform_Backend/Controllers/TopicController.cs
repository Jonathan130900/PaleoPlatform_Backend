using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TopicsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Create a new topic (only Admins and Mods can create)
        [HttpPost]
        [Authorize(Roles = "Amministratore,Moderatore")]
        public async Task<IActionResult> Create([FromBody] TopicCreateDto topicDto)
        {
            if (topicDto == null || string.IsNullOrEmpty(topicDto.Nome))
                return BadRequest("Nome del topic non valido.");

            var topic = new Topics
            {
                Nome = topicDto.Nome
            };

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            // Return a simplified response with just the topic ID and name
            return CreatedAtAction(nameof(Get), new { id = topic.Id }, new
            {
                topic.Id,
                topic.Nome
            });
        }


        // Get a topic by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
                return NotFound("Topic non trovato");

            return Ok(topic);
        }
    }
}