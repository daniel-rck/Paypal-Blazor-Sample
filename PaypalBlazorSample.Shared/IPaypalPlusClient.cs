using System.Threading.Tasks;

namespace PaypalBlazorSample.Shared
{
    public interface IPayPalPlusClient
    {
        string RedirectUrl { get; set; }
        Task CreatePaymentAsync(decimal totalAmount, string bookingDescription, string articleDescription, string internalReference);
    }
}
