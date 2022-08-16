using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.SignalR
{
    public class GameHub : Hub
    {
        private readonly ILobbyService _lobbyService;
        private readonly IPendingPlayerService _pendingPlayerService;

        public GameHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
        }

        public async Task JoinLobby(string userId, string pin)
        {
            var pp = _lobbyService.JoinLobby(userId, pin);
            if (pp?.Lobby.Id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby);
                await Clients.Group(pp.Lobby.Id).SendAsync("lobbyPlayerListUpdate", _pendingPlayerService.GetByLobbyId(pp.Lobby.Id));
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
                await Clients.Caller.SendAsync("receiveLobby", lobby);
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
                await Clients.Group(ll.LobbyId).SendAsync("lobbyPlayerListUpdate", _pendingPlayerService.GetByLobbyId(ll.LobbyId));
            }
        }

        public async Task StartGame(StartGameDtos sg)
        {
            var lobby = _lobbyService.GetById(sg.LobbyId);
            if (lobby is not null)
            {
                await Clients.Group(lobby.Id).SendAsync("gameStarting");
            }
            
        }
        
    }
}