using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TilePackController:ControllerBase
    {
        private readonly ITilePackService _tilePackService;

        public TilePackController(ITilePackService tilePackService)
        {
            _tilePackService = tilePackService;
        }
        
        [HttpGet]
        public ActionResult<List<TilePack>> GetAll()
        {
            return _tilePackService.GetAll();
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackTile))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<TilePack> GetById(string id)
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

        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TilePack))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TilePack> Create(NewTilePackDto toCreate)
        {
            try
            {
                var created = _tilePackService.Create(new TilePack(null, toCreate.Name, toCreate.PicUrl));
                return CreatedAtAction(nameof(GetById), new{id = created.Id}, created);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            
        }
    }
}