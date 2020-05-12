using PayPal.v1.Payments;

namespace PaypalBlazorSample.Shared
{
    public class PaypalTransaction
    {
        public string InternalReference { get; set; }
        public decimal TotalAmount { get; set; }
        public string BookingDescription { get; set; }
        public string ArticleDescription { get; set; }
        public string RedirectUrl { get; set; }
    }
}
