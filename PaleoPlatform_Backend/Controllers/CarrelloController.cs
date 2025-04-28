using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PaleoPlatform_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarrelloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _frontendDomain = "http://localhost:3000"; // <-- you can move this to config later

        public CarrelloController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        }

        [HttpGet]
        public async Task<ActionResult<CarrelloReadDto>> GetCarrello()
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                    .ThenInclude(i => i.Prodotto)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId);

            if (carrello == null)
                return NotFound("Carrello non trovato.");

            var carrelloDto = _mapper.Map<CarrelloReadDto>(carrello);
            return Ok(carrelloDto);
        }

        [HttpPost("aggiungi")]
        public async Task<IActionResult> AggiungiAlCarrello([FromBody] CarrelloItemDto itemDto)
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId)
                ?? new Carrello { UtenteId = utenteId };

            var prodotto = await _context.Prodotti.FindAsync(itemDto.ProdottoId);
            if (prodotto == null)
                return NotFound("Prodotto non trovato.");

            var itemEsistente = carrello.Items.FirstOrDefault(i => i.ProdottoId == itemDto.ProdottoId);
            if (itemEsistente != null)
            {
                itemEsistente.Quantità += itemDto.Quantità;
            }
            else
            {
                carrello.Items.Add(new CarrelloItem
                {
                    UtenteId = utenteId,
                    ProdottoId = itemDto.ProdottoId,
                    Quantità = itemDto.Quantità
                });
            }

            _context.Carrelli.Update(carrello); // In case it's a new carrello
            await _context.SaveChangesAsync();

            return Ok("Prodotto aggiunto al carrello.");
        }

        [HttpPost("rimuovi")]
        public async Task<IActionResult> RimuoviDalCarrello([FromBody] CarrelloItemDto itemDto)
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId);

            if (carrello == null)
                return NotFound("Carrello non trovato.");

            var item = carrello.Items.FirstOrDefault(i => i.ProdottoId == itemDto.ProdottoId);
            if (item == null)
                return NotFound("Prodotto non presente nel carrello.");

            carrello.Items.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Prodotto rimosso dal carrello.");
        }

        [HttpPost("svuota")]
        public async Task<IActionResult> SvuotaCarrello()
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId);

            if (carrello == null)
                return NotFound("Carrello non trovato.");

            carrello.Items.Clear();
            await _context.SaveChangesAsync();

            return Ok("Carrello svuotato.");
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutCarrello()
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                    .ThenInclude(i => i.Prodotto)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId);

            if (carrello == null || !carrello.Items.Any())
                return BadRequest("Il carrello è vuoto o non trovato.");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = carrello.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Prodotto.Nome,
                        },
                        UnitAmount = (long)(item.Prodotto.Prezzo * 100),
                    },
                    Quantity = item.Quantità,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{_frontendDomain}/success",
                CancelUrl = $"{_frontendDomain}/cancel",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { url = session.Url });
        }

        [HttpPost("checkout-success")]
        public async Task<IActionResult> CheckoutSuccess()
        {
            var utenteId = GetUserId();
            if (string.IsNullOrEmpty(utenteId))
                return Unauthorized("Utente non autenticato.");

            var carrello = await _context.Carrelli
                .Include(c => c.Items)
                    .ThenInclude(i => i.Prodotto)
                .FirstOrDefaultAsync(c => c.UtenteId == utenteId);

            if (carrello == null)
                return NotFound("Carrello non trovato.");

            foreach (var item in carrello.Items)
            {
                item.Prodotto.QuantitaDisponibile = Math.Max(item.Prodotto.QuantitaDisponibile - item.Quantità, 0);
            }

            carrello.Items.Clear();
            await _context.SaveChangesAsync();

            return Ok("Pagamento completato. Prodotti aggiornati.");
        }
    }
}
