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
    public class LobbyController : ControllerBase
    {
        private readonly ILobbyService _lobbyService;
        private readonly IPendingPlayerService _pendingPlayerService;

        public LobbyController(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
        }

        [HttpGet("{lobbyId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LobbyForPlayerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Lobby> GetById(string lobbyId)
        {
            var lobby = _lobbyService.GetById(lobbyId);
            if (lobby?.Id is null) return NotFound("lobby not found");
            return Ok(lobby);
        }

        [HttpGet(nameof(GetPlayersInLobby))]
        public ActionResult<List<PendingPlayer>> GetPlayersInLobby(string lobbyId)
        {
            return _pendingPlayerService.GetByLobbyId(lobbyId);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Lobby> Create()
        {
            try
            {
                var l = _lobbyService.Create(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return CreatedAtAction(nameof(GetById), new {lobbyId = l}, l);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpDelete(nameof(CloseLobby))]
        public ActionResult CloseLobby(string lobbyId)
        {
            try
            {
                _lobbyService.CloseLobby(lobbyId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}