using Microsoft.AspNetCore.Mvc;
using PaypalBlazorSample.Shared;
using System;
using System.Threading.Tasks;

namespace PaypalBlazorSample.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPayPalPlusApiService paypalService;

        public PaymentController(IPayPalPlusApiService paypal)
        {
            paypalService = paypal;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaypalTransaction transaction)
        {
            if (string.IsNullOrEmpty(transaction.InternalReference))
            {
                return NotFound("InternalReference is null or empty!");
            }

            // Saving transaction.InternalReference to db here...
            var payment = await paypalService.CreatePaymentAsync(transaction);
            if (payment == null)
            {
                return BadRequest("InternalReference is invalid");
            }
            else
            {
                return Ok(payment);
            }
        }

        [HttpGet]
        [Route("success")]
        public async Task<IActionResult> ReportPaymentSuccess([FromQuery] string bookingNumber, string guid, string verification)
        {
            // Save booking-stuff when payment was successful
            throw new NotImplementedException();            
        }

        [HttpGet]
        [Route("cancel")]
        public async Task<IActionResult> ReportPaymentCanceled([FromQuery] string bookingNumber, string guid, string verification)
        {
            // Save booking-stuff when payment was canceled            
            throw new NotImplementedException();
        }
    }
}
