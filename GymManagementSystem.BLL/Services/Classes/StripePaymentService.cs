using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly string _webhookSecret;

        public StripePaymentService(IConfiguration configuration)
        {
            var secretKey = configuration["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe:SecretKey is not configured.");
            _webhookSecret = configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");
            Stripe.StripeConfiguration.ApiKey = secretKey;
        }

        public async Task<string> CreateCheckoutSessionAsync(
            int planId,
            int memberId,
            string planName,
            decimal amount,
            string currency,
            string memberEmail,
            string successUrl,
            string cancelUrl,
            CancellationToken ct = default)
        {
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
                {
                    new()
                    {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100),
                            Currency = currency.ToLower(),
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"FITGYM - {planName}",
                                Description = $"Gym membership: {planName}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                CustomerEmail = memberEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "planId", planId.ToString() },
                    { "memberId", memberId.ToString() },
                    { "planName", planName },
                    { "amount", amount.ToString() },
                    { "currency", currency }
                },
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);
            return session.Url ?? throw new InvalidOperationException("Failed to create Stripe checkout session.");
        }

        public Task<bool> VerifyWebhookAsync(string jsonBody, string stripeSignature, CancellationToken ct = default)
        {
            try
            {
                Stripe.EventUtility.ConstructEvent(
                    jsonBody,
                    stripeSignature,
                    _webhookSecret,
                    tolerance: 300,
                    throwOnApiVersionMismatch: false);

                return Task.FromResult(true);
            }
            catch (Stripe.StripeException)
            {
                return Task.FromResult(false);
            }
        }

        public Task<(int planId, int memberId, decimal amount, string currency, string paymentIntentId)> ParseWebhookEventAsync(string jsonBody, CancellationToken ct = default)
        {
            var stripeEvent = Stripe.EventUtility.ConstructEvent(
                jsonBody,
                null,
                _webhookSecret,
                tolerance: 300,
                throwOnApiVersionMismatch: false);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                if (session?.Metadata is not null && session.PaymentIntentId is not null)
                {
                    var planId = int.Parse(session.Metadata["planId"]);
                    var memberId = int.Parse(session.Metadata["memberId"]);
                    var amount = decimal.Parse(session.Metadata["amount"]);
                    var currency = session.Metadata["currency"];

                    return Task.FromResult((planId, memberId, amount, currency, session.PaymentIntentId));
                }
            }

            throw new InvalidOperationException($"Unexpected Stripe event type: {stripeEvent.Type}");
        }
    }
}
