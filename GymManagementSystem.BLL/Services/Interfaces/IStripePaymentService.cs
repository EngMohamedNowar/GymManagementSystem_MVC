namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IStripePaymentService
    {
        Task<string> CreateCheckoutSessionAsync(int planId, int memberId, string planName, decimal amount, string currency, string memberEmail, string successUrl, string cancelUrl, CancellationToken ct = default);
        Task<bool> VerifyWebhookAsync(string jsonBody, string stripeSignature, CancellationToken ct = default);
        Task<(int planId, int memberId, decimal amount, string currency, string paymentIntentId)> ParseWebhookEventAsync(string jsonBody, CancellationToken ct = default);
    }
}
