using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;
using Stripe;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TilePackController : ControllerBase
    {
        private readonly PriceService _priceService;
        private readonly ProductService _productService;
        private readonly ITilePackService _tilePackService;

        public TilePackController(ITilePackService tilePackService, PriceService priceService,
            ProductService productService)
        {
            _tilePackService = tilePackService;
            _priceService = priceService;
            _productService = productService;
        }

        [HttpGet]
        public ActionResult<List<TilePack>> GetAll()
        {
            try
            {
                var tps = _tilePackService.GetAll(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<TilePackDto> tpDtOs = new();
                foreach (TilePack tp in tps)
                    if (!string.IsNullOrEmpty(tp.PriceStripe))
                    {
                        Price price = _priceService.Get(tp.PriceStripe);
                        tpDtOs.Add(new TilePackDto(tp) {Price = price.UnitAmount});
                    }
                    else
                    {
                        tpDtOs.Add(new TilePackDto(tp));
                    }

                return Ok(tpDtOs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TilePackDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TilePackDto> GetById([Required] [StringLength(50, MinimumLength = 36)] string id)
        {
            try
            {
                var tp = _tilePackService.GetById(id);
                Price? price = null;
                if (!string.IsNullOrEmpty(tp.PriceStripe))
                {
                    price = _priceService.Get(tp.PriceStripe);
                }
                return Ok(new TilePackDto(tp){Price = price?.UnitAmount ?? null});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
        }
        

        [HttpGet(nameof(GetDefault))]
        public ActionResult<TilePack> GetDefault()
        {
            return _tilePackService.GetDefault();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TilePackDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TilePackDto> Create(TilePackDto toCreate)
        {
            try
            {
                var tp = toCreate.ToTilePack();

                
                    
                    var product = _productService.Create(new ProductCreateOptions {Name = toCreate.Name});
                    var price = _priceService.Create(new PriceCreateOptions
                        {UnitAmount = toCreate.Price ?? 0, Currency = "usd", Product = product.Id});
                    tp.PriceStripe = price.Id;
                


                var created = _tilePackService.Create(tp);
                return CreatedAtAction(nameof(GetById), new {id = created.Id}, new TilePackDto(created));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Update(TilePackDto toUpdate)
        {
            try
            {
                var old =
                    _tilePackService.GetById(toUpdate.Id ??
                                             throw new Exception(
                                                 "You need to provide an id of the tilePack you want to update"));

                var updated = toUpdate.ToTilePack();
                if (!string.IsNullOrEmpty(old.PriceStripe) && toUpdate.Price is >= 0)
                {
                    var oldPrice = _priceService.Get(old.PriceStripe);
                    if (oldPrice.UnitAmount != toUpdate.Price)
                    {
                        var newPrice = _priceService.Create(new PriceCreateOptions
                            {UnitAmount = toUpdate.Price, Currency = "usd", Product = oldPrice.ProductId});
                        _priceService.Update(oldPrice.Id, new PriceUpdateOptions {Active = false});
                        updated.PriceStripe = newPrice.Id;
                    }
                }
                else if (toUpdate.Price is >=0)
                {
                    var product = _productService.Create(new ProductCreateOptions {Name = toUpdate.Name});
                    var price = _priceService.Create(new PriceCreateOptions
                        {UnitAmount = toUpdate.Price, Currency = "usd", Product = product.Id});
                    updated.PriceStripe = price.Id;
                }

                _tilePackService.Update(updated);


                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public ActionResult Delete(string packId)
        {
            try
            {
                _tilePackService.Delete(packId);
                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}