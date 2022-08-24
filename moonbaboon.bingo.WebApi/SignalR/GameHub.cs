using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.SignalR
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly ILobbyService _lobbyService;
        private readonly IPendingPlayerService _pendingPlayerService;
        private readonly IGameService _gameService;


        public GameHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService, IGameService gameService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
            _gameService = gameService;
        }

        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("PopUpError", message);
        }

        private string GetUserId(ClaimsPrincipal? user)
        {
            if (user== null) throw new Exception("Could not get userId from Context");
            string u = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Could not get userId from Context");
            return u;
        }

        public async Task JoinLobby(string pin)
        {
            PendingPlayer? pp; 
            try
            {
                pp = _lobbyService.JoinLobby(GetUserId(Context.User), pin);
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
            if (pp.Lobby.Id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby);
                var playerList = _pendingPlayerService.GetByLobbyId(pp.Lobby.Id).Select(p => new PendingPlayerDto(p)).ToList();

                await Clients.Group(pp.Lobby.Id).SendAsync("lobbyPlayerListUpdate", playerList);
            }
            else
            {
                await SendError("Lobby id is null");
            }
            
        }

        public async Task CreateLobby(string hostId)
        {
            var lobby = _lobbyService.GetByHostId(hostId);
            if (lobby?.Id != null)
            {
                _lobbyService.CloseLobby(lobby.Id, hostId);
            }
            lobby = _lobbyService.Create(new Lobby(hostId));
            if (lobby?.Id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id);
                List<PendingPlayerDto> playerlist = new();
                foreach (var player in _pendingPlayerService.GetByLobbyId(lobby.Id))
                {
                    playerlist.Add(new PendingPlayerDto(player));
                }
                await Clients.Caller.SendAsync("receiveLobby", lobby);
                await Clients.Group(lobby.Id).SendAsync("lobbyPlayerListUpdate", playerlist);
            }

        }

        public async Task StartGame(StartGameDtos sg)
        {
            var lobby = _lobbyService.GetById(sg.LobbyId);
            if (lobby?.Id is not null)
            {
                if (lobby.Host == GetUserId(Context.User))
                {
                    try
                    {
                        var game = _gameService.NewGame(lobby);
                        if (game?.Id != null)
                        {
                            await Clients.Group(lobby.Id).SendAsync("gameStarting", game.Id);
                        }
                    }
                    catch (Exception e)
                    {
                        await Clients.Group(lobby.Id).SendAsync("LobbyError",e.Message);
                    }
                }
            }
        }

        public async Task CloseLobby(CloseLobbyDto cl)
        {
            if (_lobbyService.CloseLobby(cl.LobbyId, cl.HostId))
            {
                await Clients.Group(cl.LobbyId).SendAsync("lobbyClosed");
            }
        }

        public async Task LeaveLobby(LeaveLobbyDto ll)
        {
            if (_lobbyService.LeaveLobby(ll.LobbyId, ll.UserId))
            {
                List<PendingPlayerDto> playerList = new();
                foreach (var player in _pendingPlayerService.GetByLobbyId(ll.LobbyId))
                {
                    playerList.Add(new PendingPlayerDto(player));
                }
                await Clients.Group(ll.LobbyId).SendAsync("lobbyPlayerListUpdate", playerList);            }
        }

    }
}