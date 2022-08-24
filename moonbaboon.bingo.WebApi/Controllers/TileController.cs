using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TileController : ControllerBase
    {
        private readonly ITileService _tileService;

        public TileController(ITileService tileService)
        {
            _tileService = tileService;
        }

        [HttpGet]
        public ActionResult<List<Tile>> GetAll()
        {
            return _tileService.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<Tile?> GetById(string id) {
            return _tileService.GetById(id);
        }
        [HttpGet(nameof(GetAboutUserById))]
        public ActionResult<List<Tile>> GetAboutUserById(string id)
        {
            return _tileService.GetAboutUserById(id);
        }
        
        [HttpGet(nameof(GetMadeByUserId))]
        public ActionResult<List<Tile>> GetMadeByUserId(string userId)
        {
            return _tileService.GetMadeByUserId(userId);
        }
        
        [Authorize]
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Tile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Tile?> Create(NewTileDto newTile)
        {
            try
            {
                var tile = _tileService.NewTile(newTile.AboutUserId, newTile.Action,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return CreatedAtAction(nameof(GetById), new {id = tile.Id}, tile);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        

        [HttpPost(nameof(Delete))]
        public ActionResult<bool> Delete(string id)
        {
            return _tileService.DeleteTile(id);
        }
    }
}
