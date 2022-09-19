using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.Models;
using Stripe;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StripeController: ControllerBase
    {
        [Authorize(Roles = nameof(Admin))]
        [HttpPost(nameof(createProduct))]
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

        [Authorize(Roles = nameof(Admin))]
        [HttpGet(nameof(GetPrice))]
        public ActionResult<long> GetPrice(string priceId)
        {
            var service = new PriceService();

            return service.Get(priceId).UnitAmount ?? 0;
        }
        
        
    }
    
}