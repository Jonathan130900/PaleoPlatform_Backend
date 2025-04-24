using Stripe;

namespace PaleoPlatform_Backend.Services
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;

        public StripeService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"]; 
        }

        public PaymentIntent CreatePaymentIntent(decimal amount)
        {
            StripeConfiguration.ApiKey = _secretKey;

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "eur",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            return service.Create(options);
        }

        public PaymentIntent ConfirmPayment(string paymentIntentId)
        {
            StripeConfiguration.ApiKey = _secretKey;

            var service = new PaymentIntentService();
            return service.Confirm(paymentIntentId);
        }
    }
}
