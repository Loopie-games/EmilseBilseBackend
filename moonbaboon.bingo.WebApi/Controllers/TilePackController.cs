﻿using System;
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
        private readonly ITilePackService _tilePackService;

        public TilePackController(ITilePackService tilePackService)
        {
            _tilePackService = tilePackService;
        }

        [HttpGet]
        public ActionResult<List<TilePack>> GetAll()
        {
            try
            {
                var tps = _tilePackService.GetAll(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var priceService = new PriceService();
                List<TilePackDto> tpDtOs = new();
                foreach (TilePack tp in tps)
                {
                    if (!string.IsNullOrEmpty(tp.PriceStripe))
                    {
                        Price price = priceService.Get(tp.PriceStripe);
                        tpDtOs.Add(new TilePackDto(tp) {Price = price.UnitAmount});
                    }
                    else
                    {
                        tpDtOs.Add(new TilePackDto(tp));
                    }
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackTile))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TilePack> GetById([Required, StringLength(50, MinimumLength = 36)]string id)
        {
            try
            {
                return Ok(_tilePackService.GetById(id));
            }
            catch (Exception e)
            {
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TilePack))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TilePack> Create(TilePack toCreate)
        {
            try
            {
                var created = _tilePackService.Create(toCreate);
                return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}