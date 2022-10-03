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
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
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
        [HttpGet(nameof(GetEnded))]
        public ActionResult<List<Game>> GetEnded()
        {
            try
            {
                return _gameService.GetEnded(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        [Authorize]
        [HttpPost]
        public ActionResult<Game> Create(CreateGameDto gameDto)
        {
            try
            {
                return _gameService.NewGame(gameDto.LobbyId,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, gameDto.TpIds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        public class CreateGameDto
        {
            public CreateGameDto(string lobbyId, string[]? tpIds)
            {
                LobbyId = lobbyId;
                TpIds = tpIds;
            }

            public string LobbyId { get; set; }
            public string[]? TpIds { get; set; }
        }
    }
}