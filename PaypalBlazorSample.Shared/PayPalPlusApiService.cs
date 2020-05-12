using BraintreeHttp;
using Microsoft.Extensions.Configuration;
using PayPal.Core;
using PayPal.v1.Payments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PaypalBlazorSample.Shared
{
    public class PayPalPlusApiService : IPayPalPlusApiService
    {
        private readonly bool isSandboxMode;
        private readonly string clientId;
        private readonly string clientSecret;

        public PayPalPlusApiService(IConfiguration configuration)
        {
            isSandboxMode = configuration["Paypal:Mode"].Equals("sandbox", StringComparison.OrdinalIgnoreCase);
            if (isSandboxMode)
            {
                clientId = configuration["Paypal:Sandbox:ClientId"];
                clientSecret = configuration["Paypal:Sandbox:ClientSecret"];
            }
            else
            {
                clientId = configuration["Paypal:Live:ClientId"];
                clientSecret = configuration["Paypal:Live:ClientSecret"];
            }
        }

        public async Task<Payment> CreatePaymentAsync(PaypalTransaction transaction)
        {
            PayPalEnvironment environment;
            if (isSandboxMode)
            {
                environment = new SandboxEnvironment(clientId, clientSecret);
            }
            else
            {
                environment = new LiveEnvironment(clientId, clientSecret);
            }
            var client = new PayPalHttpClient(environment);
            try
            {
                var payment = CreatePayment(transaction);
                var request = new PaymentCreateRequest().RequestBody(payment);

                var response = await client.Execute(request);
                var statusCode = response.StatusCode;
                var result = response.Result<Payment>();
                return result;
            }
            catch (HttpException httpException)
            {
                var statusCode = httpException.StatusCode;
                var debugId = httpException.Headers.GetValues("PayPal-Debug-Id").FirstOrDefault();
                return null;
            }
        }

        public string GetReferenceWithVerification(string entityName, string number, Guid guid)
        {
            using (var sha512 = new SHA512Managed())
            {
                var data = System.Text.Encoding.UTF8.GetBytes(string.Concat(number, guid.ToString("D"), "no-random-salt"));
                string verification = Convert.ToBase64String(sha512.ComputeHash(data)).Replace("+", "1").Replace("/", "2").Replace("=", "0");
                return $"{entityName}.{number}.{guid:N}.{verification}";
            }
        }

        private Payment CreatePayment(PaypalTransaction transaction)
        {
            decimal taxRatio = 0.19M;
            decimal taxAmount = Math.Round(transaction.TotalAmount * taxRatio, 2, MidpointRounding.AwayFromZero);
            decimal amountWithoutTaxes = transaction.TotalAmount - taxAmount;

            var split = transaction.InternalReference.Split('.');
            string baseUrl = $"{transaction.RedirectUrl.TrimEnd('/')}/{split[0]}/{split[1]}?guid={split[2]}";
            string successUrl = $"{baseUrl}&payment=success&verification={split[3]}";
            string cancelUrl = $"{baseUrl}&payment=cancel";

            var usCulture = new CultureInfo("en-US");
            var payment = new Payment()
            {
                Intent = "sale",
                Transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        Amount = new Amount
                        {
                            Total = transaction.TotalAmount.ToString("N2", usCulture),
                            Currency = "EUR",
                            Details = new AmountDetails
                            {
                                Subtotal = amountWithoutTaxes.ToString("N2", usCulture),
                                Tax = taxAmount.ToString("N2", usCulture)
                            }

                        },
                        Description = transaction.BookingDescription,
                        ItemList = new ItemList
                        {
                            Items= new List<Item>
                            {
                                new Item
                                {
                                    Price = amountWithoutTaxes.ToString("N2", usCulture),
                                    Quantity = "1",
                                    Description = transaction.ArticleDescription,
                                    Currency = "EUR"
                                }
                            }
                        }
                    }
                },
                RedirectUrls = new RedirectUrls
                {
                    CancelUrl = cancelUrl,
                    ReturnUrl = successUrl
                },
                Payer = new Payer
                {
                    PaymentMethod = "paypal"
                }
            };
            return payment;
        }
    }
}
