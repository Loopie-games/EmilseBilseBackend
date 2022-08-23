using System;
using System.Collections.Generic;
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
        private readonly IBoardService _boardService;


        public override Task OnConnectedAsync()
        {
            //Console.WriteLine(Context.User.Identity.Name);
            return base.OnConnectedAsync();
        }

        public GameHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService, IGameService gameService, IBoardService boardService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
            _gameService = gameService;
            _boardService = boardService;
        }

        public async Task JoinLobby(string userId, string pin)
        {
            var pp = _lobbyService.JoinLobby(userId, pin);
            if (pp?.Lobby.Id != null)
            {
                
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby);
                List<PendingPlayerDto> playerlist = new();
                foreach (var player in _pendingPlayerService.GetByLobbyId(pp.Lobby.Id))
                {
                    playerlist.Add(new PendingPlayerDto(player));
                }
                
                await Clients.Group(pp.Lobby.Id).SendAsync("lobbyPlayerListUpdate", playerlist);
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
                if (lobby.Host == Context.User.FindFirst(ClaimTypes.NameIdentifier).Value)
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
                List<PendingPlayerDto> playerlist = new();
                foreach (var player in _pendingPlayerService.GetByLobbyId(ll.LobbyId))
                {
                    playerlist.Add(new PendingPlayerDto(player));
                }
                await Clients.Group(ll.LobbyId).SendAsync("lobbyPlayerListUpdate", playerlist);            }
        }

    }
    
    public class UserIdProvider: IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}