using Stripe;

namespace PaleoPlatform_Backend.Services
{
    public interface IStripeService
    {
        PaymentIntent CreatePaymentIntent(decimal amount);
        PaymentIntent ConfirmPayment(string paymentIntentId); 
    }
}
