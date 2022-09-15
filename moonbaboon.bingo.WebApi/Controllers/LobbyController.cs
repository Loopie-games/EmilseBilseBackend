﻿using System.Collections.Generic;
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
        private readonly IUserService _userService;

        public LobbyController(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService,
            IUserService userService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
            _userService = userService;
        }

        [HttpGet("{lobbyId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LobbyForPlayerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<LobbyForPlayerDto> GetById(string lobbyId)
        {
            var lobby = _lobbyService.GetById(lobbyId);
            if (lobby?.Id is null || lobby.Pin is null) return NotFound("lobby not found");
            var host = _userService.GetById(lobby.Host);
            if (host is null) return NotFound("host not found");
            host.Id = null;
            return Ok(new LobbyForPlayerDto(lobby.Id, lobby.Pin, host));
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
            return _lobbyService.Create(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [Authorize]
        [HttpDelete(nameof(CloseLobby))]
        public ActionResult<bool> CloseLobby(string lobbyId)
        {
            return _lobbyService.CloseLobby(lobbyId, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}