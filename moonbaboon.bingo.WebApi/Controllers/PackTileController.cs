﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
                Console.WriteLine(e);
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
                Console.Write(e);
                return NotFound(e.Message);
            }
        }

        [HttpDelete(nameof(ClearPack))]
        public ActionResult<bool> ClearPack(string id)
        {
            try
            {
                return _packTileService.Clear(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost(nameof(AddToTilePack))]
        public ActionResult<PackTile> AddToTilePack(PackTileEntity pt)
        {
            try
            {
                var created = _packTileService.Create(pt);
                return CreatedAtAction(nameof(GetById), new {created.Id}, created);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest(e.Message);
            }
        }
    }
}