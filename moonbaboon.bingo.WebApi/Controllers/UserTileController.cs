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
    [ApiController]
    [Route("[controller]")]
    public class UserTileController : ControllerBase
    {
        private readonly IUserTileService _userTileService;

        public UserTileController(IUserTileService userTileService)
        {
            _userTileService = userTileService;
        }

        [HttpGet]
        public ActionResult<List<UserTile>> GetAll()
        {
            return _userTileService.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<UserTile?> GetById(string id)
        {
            return _userTileService.GetById(id);
        }

        [HttpGet(nameof(GetAboutUserById))]
        public ActionResult<List<UserTile>> GetAboutUserById(string id)
        {
            return _userTileService.GetAboutUserById(id);
        }

        [HttpGet(nameof(GetMadeByUserId))]
        public ActionResult<List<UserTile>> GetMadeByUserId(string userId)
        {
            return _userTileService.GetMadeByUserId(userId);
        }

        [Authorize]
        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserTile))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserTile?> Create(NewTileDto newTile)
        {
            try
            {
                var tile = _userTileService.NewTile(newTile.AboutUserId, newTile.Action,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return CreatedAtAction(nameof(GetById), new {id = tile.Id}, tile);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}