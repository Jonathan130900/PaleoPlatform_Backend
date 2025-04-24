using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using PaleoPlatform_Backend.Models;
using PaleoPlatform_Backend.Models.DTOs;
using PaleoPlatform_Backend.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PaleoPlatform_Backend.Services;

[Route("api/[controller]")]
[ApiController]
public class BigliettiController : ControllerBase 
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IStripeService _stripeService;

    public BigliettiController(ApplicationDbContext context, IStripeService stripeService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _stripeService = stripeService;
        _userManager = userManager;
    }


    // Endpoint to create a ticket and initiate payment
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTicket(BigliettoCreateDto dto)
    {
        // Get the current user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Check if event exists
        var evento = await _context.Eventi
            .FirstOrDefaultAsync(e => e.Id == dto.EventoId);

        if (evento == null)
        {
            return NotFound("Evento non trovato.");
        }

        // Check if there are still available tickets
        if (evento.PostiDisponibili <= 0)
        {
            return BadRequest("Non ci sono più posti disponibili per questo evento.");
        }

        // Create the ticket (without payment for now)
        var biglietto = new Biglietto
        {
            EventoId = dto.EventoId,
            UtenteId = userId,
            Prezzo = dto.Prezzo,
            Pagato = false,
            DataAcquisto = DateTime.Now
        };

        _context.Biglietti.Add(biglietto);
        await _context.SaveChangesAsync();

        // Create a Stripe PaymentIntent
        var paymentIntent = _stripeService.CreatePaymentIntent(dto.Prezzo);

        // Return the client secret to the frontend for Stripe
        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }

    // Endpoint to confirm payment (called after frontend processes payment)
    [HttpPost("confirm-payment/{ticketId}")]
    [Authorize]
    public async Task<IActionResult> ConfirmPayment(int ticketId, [FromBody] string paymentIntentId)
    {
        var ticket = await _context.Biglietti
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            return NotFound("Ticket non trovato.");
        }

        // Confirm the payment with Stripe
        var paymentIntent = _stripeService.ConfirmPayment(paymentIntentId);

        // If payment was successful, update the ticket status to paid
        if (paymentIntent.Status == "succeeded")
        {
            ticket.Pagato = true;
            await _context.SaveChangesAsync();
            return Ok("Pagamento confermato.");
        }

        return BadRequest("Pagamento non riuscito.");
    }
}
