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
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Game))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Game> GetById(string id)
        {
            try
            {
                var game = _gameService.GetById(id);
                Console.Write("H: "+game.Host);
                return Ok(game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TilePackDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Game> Create(GameDtos.CreateGameDto gameDto)
        {
            try
            { 
                var gameId =_gameService.NewGame(gameDto.LobbyId,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, gameDto.TpIds);
                Console.WriteLine(gameId);
                return _gameService.GetById(gameId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}