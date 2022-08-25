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

        /// <summary>
        /// Send en Error message to the client
        /// </summary>
        /// <param name="message">Error Message</param>
        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("PopUpError", message);
        }
        
        /// <summary>
        /// Gets UserId From Authorized user 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>userId</returns>
        /// <exception cref="Exception">if user is null</exception>
        /// <exception cref="InvalidOperationException">if user id cant be found</exception>
        private string GetUserId(HubCallerContext context)
        {
            if (context.User== null) throw new Exception("Could not get user from Context");
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new InvalidOperationException("Could not get userId from Context");

        }

        /// <summary>
        /// Adds Authorized user to lobby and sends updates to clients
        /// </summary>
        /// <param name="pin">Pin for lobby</param>
        /// <exception cref="Exception">If User cant be added.</exception>
        
        public async Task JoinLobby(string pin)
        {
            try
            {
                var pp = _lobbyService.JoinLobby(GetUserId(Context), pin);
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id!);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby);
                var playerList = _pendingPlayerService.GetByLobbyId(pp.Lobby.Id!).Select(p => new PendingPlayerDto(p)).ToList();
                await Clients.Group(pp.Lobby.Id!).SendAsync("lobbyPlayerListUpdate", playerList);
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }
        
        

        public async Task CreateLobby(string hostId)
        {
            try
            {
                var lobby = _lobbyService.GetByHostId(hostId);
                //if user is already host for a lob
                if (lobby is not null)
                {
                    _lobbyService.CloseLobby(lobby.Id!, hostId);
                }
                lobby = _lobbyService.Create(new Lobby(hostId));
                if (lobby?.Id != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id);
                    List<PendingPlayerDto> playerList = new();
                    foreach (var player in _pendingPlayerService.GetByLobbyId(lobby.Id))
                    {
                        playerList.Add(new PendingPlayerDto(player));
                    }
                    await Clients.Caller.SendAsync("receiveLobby", lobby);
                    await Clients.Group(lobby.Id).SendAsync("lobbyPlayerListUpdate", playerList);
                }
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }

        public async Task StartGame(StartGameDtos sg)
        {
            var lobby = _lobbyService.GetById(sg.LobbyId);
            if (lobby?.Id is not null)
            {
                if (lobby.Host == GetUserId(Context))
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