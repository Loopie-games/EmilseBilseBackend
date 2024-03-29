﻿using System;
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
                return Ok(game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
        }
        
        [Authorize]
        [HttpGet(nameof(GetSavedGames))]
        public ActionResult<Game> GetSavedGames()
        {
            try
            {
                var games = _gameService.GetSavedGames(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return Ok(games);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
        }
        

        [Authorize]
        [HttpGet(nameof(GetPlayers) + "/{gameId}")]
        public ActionResult<List<User>> GetPlayers(string gameId)
        {
            try
            {
                return _gameService.GetPlayers(gameId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteGame(string gameId)
        {
            try
            {
                _gameService.Delete(gameId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPut(nameof(SetName))]
        public ActionResult<Game> SetName(GameDtos.GameNameChangeDto gN)
        {
            try
            {
                var game = new GameEntity(_gameService.GetById(gN.GameId));
                game.Name = gN.Name;
                _gameService.Update(game);
                return CreatedAtAction(nameof(GetById), new {id = gN.GameId}, gN);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        } 

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Game))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> Create(GameDtos.CreateGameDto gameDto)
        {
            try
            {
                var gameId = _gameService.NewOG(gameDto.LobbyId,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, gameDto.TpIds);
                var game = new GameEntity(_gameService.GetById(gameId));
                game.Name = gameDto.Name;
                _gameService.Update(game);
                
                return gameId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("/FFA")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> CreateFreeForAll(GameDtos.CreateGameDto gameDto)
        {
            try
            {
                var gameId = _gameService.NewFreeForAll(gameDto.LobbyId,
                    HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, gameDto.TpIds);
                var game = new GameEntity(_gameService.GetById(gameId));
                game.Name = gameDto.Name;
                _gameService.Update(game);
                return gameId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        
        [Authorize]
        [HttpPost("/Shared")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Game))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<string> CreateShared(GameDtos.CreateGameDto gameDto)
        {
            try
            {
                var gameId = _gameService.NewShared(gameDto.LobbyId,HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, gameDto.TpIds);
                var temp = _gameService.GetById(gameId);
                var game = new GameEntity(temp);
                game.Name = gameDto.Name;
                _gameService.Update(game);
                return Ok(gameId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}