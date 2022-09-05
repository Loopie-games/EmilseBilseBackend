using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController: ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IAuthService _authService;

        public GameController(IGameService gameService, IAuthService authService)
        {
            _gameService = gameService;
            _authService = authService;
        }
        
        [HttpGet("{id}")]
        public ActionResult<Game?> GetById(string id)
        {
            try
            { 
                var game = _gameService.GetById(id);
                return Ok(game);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [Authorize]
        [HttpGet(nameof(GetPlayers) + "/{gameId}")]
        public ActionResult<List<UserSimple>> GetPlayers(string gameId)
        {
            try
            {
                return _gameService.GetPlayers(gameId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        public ActionResult<bool> DeleteGame(string gameId)
        {
            try
            {
                return Ok(_gameService.Delete(gameId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #region Not in use - remove Nonaction attribute if needed again
           
        [NonAction]
        [HttpPost(nameof(Create))]
        public ActionResult<Game?> Create(string hostId)
        {   
            return _gameService.Create(hostId);
        }

        #endregion
        
    }
}