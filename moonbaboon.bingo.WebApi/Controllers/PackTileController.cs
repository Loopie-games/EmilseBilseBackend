using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class PackTileController : ControllerBase
    {
        private readonly IPackTileService _packTileService;

        public PackTileController(IPackTileService packTileService)
        {
            _packTileService = packTileService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackTile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PackTile> GetById(string id)
        {
            try
            {
                return Ok(_packTileService.GetById(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet(nameof(GetByPackId) + "/{packId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PackTile))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<PackTile>> GetByPackId(string packId)
        {
            try
            {
                return _packTileService.GetByPackId(packId);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PackTile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PackTile> Create(NewPackTileDto toCreate)
        {
            try
            {
                var created = _packTileService.Create(toCreate.Action, toCreate.PackId);
                return CreatedAtAction(nameof(GetById), new {id = created.Id}, created);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet(nameof(GetTilesUsedInPacks))]
        public ActionResult<List<Tile>> GetTilesUsedInPacks()
        {
            return _packTileService.GetTilesUsedInPacks();
        }

        [HttpPost(nameof(AddToTilePack))]
        public ActionResult<PackTile> AddToTilePack(PackTileEntity pt)
        {
            try
            {
                return _packTileService.AddToPack(pt);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest();
            }
        }
        
        public class AddTilesToPackDto
        {
            public AddTilesToPackDto(string[] toAdd, string packId)
            {
                ToAdd = toAdd;
                PackId = packId;
            }

            public string[] ToAdd { get; set; }
            public string PackId { get; set; }
        }
    }
}
