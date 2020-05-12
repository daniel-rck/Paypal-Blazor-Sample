using PayPal.v1.Payments;
using System;
using System.Threading.Tasks;

namespace PaypalBlazorSample.Shared
{
    public interface IPayPalPlusApiService
    {
        string GetReferenceWithVerification(string entityName, string number, Guid guid);
        Task<Payment> CreatePaymentAsync(PaypalTransaction transaction);
    }
}
