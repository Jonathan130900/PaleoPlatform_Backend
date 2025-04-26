using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Models;

namespace PaleoPlatform_Backend.Controllers
{
    [Authorize(Roles = "Amministratore,Moderatore")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProdottiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProdottiController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdottoReadDto>>> GetProdotti()
        {
            var prodotti = await _context.Prodotti.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ProdottoReadDto>>(prodotti));
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdottoReadDto>> GetProdotto(int id)
        {
            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto == null) return NotFound();
            return Ok(_mapper.Map<ProdottoReadDto>(prodotto));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProdotto([FromForm] ProdottoCreateDto dto)
        {
            var prodotto = _mapper.Map<Prodotto>(dto);

            if (dto.Immagine != null && dto.Immagine.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "prodotti");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Immagine.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Immagine.CopyToAsync(stream);
                }

                prodotto.ImmagineUrl = Path.Combine("uploads", "prodotti", fileName).Replace("\\", "/");
            }

            _context.Prodotti.Add(prodotto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdotto), new { id = prodotto.Id }, _mapper.Map<ProdottoReadDto>(prodotto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProdotto(int id, [FromForm] ProdottoCreateDto dto, IFormFile immagine)
        {
            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto == null) return NotFound();

            _mapper.Map(dto, prodotto);

            if (immagine != null && immagine.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "prodotti");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(immagine.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await immagine.CopyToAsync(stream);
                }

                prodotto.ImmagineUrl = Path.Combine("uploads", "prodotti", fileName).Replace("\\", "/");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProdotto(int id)
        {
            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto == null) return NotFound();

            _context.Prodotti.Remove(prodotto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
