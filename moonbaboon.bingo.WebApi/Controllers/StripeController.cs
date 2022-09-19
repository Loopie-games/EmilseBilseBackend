using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController: ControllerBase
    {
        [HttpPost]
        public ActionResult<Price> createProduct(string name, long priceG)
        {
            var optionsProduct = new ProductCreateOptions
            {
                Name = name
            };
            var serviceProduct = new ProductService();
            Product product = serviceProduct.Create(optionsProduct);
            
            var optionsPrice = new PriceCreateOptions
            {
                UnitAmount = priceG,
                Currency = "usd",
                Product = product.Id
            };
            var servicePrice = new PriceService();
            Price price = servicePrice.Create(optionsPrice);
            return price;
        }
    }
    
}