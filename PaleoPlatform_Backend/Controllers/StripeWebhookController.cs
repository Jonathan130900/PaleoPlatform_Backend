using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using PaleoPlatform_Backend.Data;
using PaleoPlatform_Backend.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PaleoPlatform_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StripeWebhookController> _logger;

        public StripeWebhookController(ApplicationDbContext context, IConfiguration configuration, ILogger<StripeWebhookController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                json = await reader.ReadToEndAsync();
            }

            var secret = _configuration["Stripe:WebhookSecret"];
            try
            {
                // Validate the Stripe signature
                var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], secret);

                // Log event type for debugging
                _logger.LogInformation($"Received Stripe event: {stripeEvent.Type}");

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandlePaymentIntentSucceeded(paymentIntent);
                        break;
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        await HandleCheckoutSessionCompleted(session);
                        break;
                    default:
                        _logger.LogWarning($"Unhandled event type: {stripeEvent.Type}");
                        return BadRequest("Unhandled event type.");
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                // Log the error and return a generic error message
                _logger.LogError($"Stripe webhook error: {ex.Message}");
                return StatusCode(500, "Webhook error");
            }
            catch (System.Exception ex)
            {
                // Catch any other unexpected errors
                _logger.LogError($"Unexpected error: {ex.Message}");
                return StatusCode(500, "Unexpected error occurred");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            var orderId = paymentIntent.Metadata["order_id"];
            var ticket = await _context.Biglietti.FindAsync(orderId);

            if (ticket != null)
            {
                ticket.Pagato = true; // Mark as paid
                ticket.StripePaymentIntentId = paymentIntent.Id;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Payment successful for ticket ID: {ticket.Id}");
            }
            else
            {
                _logger.LogWarning($"PaymentIntent succeeded, but no ticket found for order ID: {orderId}");
            }
        }

        private async Task HandleCheckoutSessionCompleted(Session session)
        {
            var orderId = session.Metadata["order_id"];
            var cart = await _context.Carrelli.FindAsync(orderId);

            if (cart != null)
            {
                cart.Pagato = true; // Mark as paid
                cart.StripeCheckoutSessionId = session.Id;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Checkout session completed for cart ID: {cart.Id}");
            }
            else
            {
                _logger.LogWarning($"Checkout session completed, but no cart found for order ID: {orderId}");
            }
        }
    }
}
