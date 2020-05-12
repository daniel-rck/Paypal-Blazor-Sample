using Microsoft.JSInterop;
using PayPal.v1.Payments;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaypalBlazorSample.Shared
{
    public class BlazorPaypalPlusClient : IPayPalPlusClient
    {
        private readonly IJSRuntime jsRuntime;
        private readonly HttpClient httpClient;

        public string RedirectUrl { get; set; }
        public BlazorPaypalPlusClient(IJSRuntime jsRuntime, HttpClient client)
        {
            this.jsRuntime = jsRuntime;
            httpClient = client;
        }

        public async Task CreatePaymentAsync(decimal totalAmount, string bookingDescription, string articleDescription, string internalReference)
        {
            var transaction = new PaypalTransaction
            {
                TotalAmount = totalAmount,
                BookingDescription = bookingDescription,
                ArticleDescription = articleDescription,
                InternalReference = internalReference,
                RedirectUrl = RedirectUrl
            };

            var response = await httpClient.PostAsync("api/payment/create", transaction.ToHttpContent());
            var payment = await response.Content.ReadAsync<Payment>();

            var approvalUrl = payment.Links.FirstOrDefault(x => x.Rel == "approval_url").Href;

            await jsRuntime.InvokeVoidAsync("blazorPaypal", approvalUrl);
        }
    }
}
