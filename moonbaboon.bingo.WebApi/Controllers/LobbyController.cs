using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController: ControllerBase
    {
        private readonly ILobbyService _lobbyService;
        private readonly IPendingPlayerService _pendingPlayerService;

        public LobbyController(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
        }

        [HttpGet("{lobbyId}")]
        public ActionResult<LobbyForUser?> GetById(string lobbyId)
        {
            return _lobbyService.GetById(lobbyId);
        }
        
        [HttpGet(nameof(GetPlayersInLobby))]
        public ActionResult<List<PendingPlayerForUser>> GetPlayersInLobby(string lobbyId)
        {
            return _pendingPlayerService.GetByLobbyId(lobbyId);
        }

        [HttpPost]
        public ActionResult<Lobby?> Create(CreateLobbyDto lobby)
        {
            return _lobbyService.Create(new Lobby(lobby.HostId));
        }
        
        [HttpDelete(nameof(CloseLobby))]
        public ActionResult<bool> CloseLobby(CloseLobbyDto cl)
        {
            return _lobbyService.CloseLobby(cl.LobbyId, cl.HostId);
        }
    }
}