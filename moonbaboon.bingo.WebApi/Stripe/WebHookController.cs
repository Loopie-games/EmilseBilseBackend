using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace moonbaboon.bingo.WebApi.Stripe
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        // This is your Stripe CLI webhook secret for testing your endpoint locally.
        private const string endpointSecret = "whsec_27021b0eaf35854c981ea970479c412b0b8e3041974c38f72c3e1b87ea2283b2";

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                    Console.WriteLine("Payment Succeeded");
                // ... handle other event types
                else
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);

                return Ok();
            }
            catch (StripeException e)
            {
                Console.Write(e);
                return BadRequest();
            }
        }
    }
}