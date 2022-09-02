using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
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

        public GameHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService, IGameService gameService, IBoardService boardService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
            _gameService = gameService;
            _boardService = boardService;
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
        private static string GetUserId(HubCallerContext context)
        {
            if (context.User == null) throw new Exception("Could not get user from Context");
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   throw new InvalidOperationException("Could not get userId from Context");
        }

        #region Game

        public async Task ConnectToGame(string gameId)
        {
            try
            {
                var board = _boardService.GetByUserAndGameId(GetUserId(Context), gameId);
                if (board is null)
                {
                    await SendError("You are not a part of this game");
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                    await Clients.Caller.SendAsync("gameConnected", board.Id);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
            
        }

        public async Task WinnerClaim(string gameId)
        {
            try
            {
                var board = _boardService.GetByUserAndGameId(GetUserId(Context), gameId);
                if (board is null)
                {
                    await SendError("You are not a part of this game");
                }
                else
                {
                    await Clients.Group(gameId).SendAsync("winnerFound", board);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region Lobby

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
                var playerList = _pendingPlayerService.GetByLobbyId(pp.Lobby.Id!).Select(p => new PendingPlayerDto(p))
                    .ToList();
                await Clients.Group(pp.Lobby.Id!).SendAsync("lobbyPlayerListUpdate", playerList);
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }


        public async Task CreateLobby()
        {
            try
            {
                var hostId = GetUserId(Context);
                
                var lobby = _lobbyService.GetByHostId(hostId);
                //if user is already host for a lobby, close the old one
                if (lobby is not null)
                {
                    _lobbyService.CloseLobby(lobby.Id!, hostId);
                }

                lobby = _lobbyService.Create(hostId);
                if (lobby?.Id != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id);
                    List<PendingPlayerDto> playerList = _pendingPlayerService.GetByLobbyId(lobby.Id)
                        .Select(player => new PendingPlayerDto(player)).ToList();

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

        [Authorize]
        public async Task StartGame(string lobbyId)
        {
            try
            {
                var game = _gameService.NewGame(lobbyId, GetUserId(Context));
                if (game.Id != null)
                {
                    await Clients.Group(lobbyId).SendAsync("gameStarting", game.Id);
                }
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }

        public async Task CloseLobby(string lobbyId)
        {
            try
            {
                if (_lobbyService.CloseLobby(lobbyId, GetUserId(Context)))
                {
                    await Clients.Group(lobbyId).SendAsync("lobbyClosed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        public async Task LeaveLobby(string lobbyId)
        {
            try
            {
                if (_lobbyService.LeaveLobby(lobbyId, GetUserId(Context)))
                {
                    List<PendingPlayerDto> playerList = _pendingPlayerService.GetByLobbyId(lobbyId)
                        .Select(player => new PendingPlayerDto(player)).ToList();
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
                    await Clients.Group(lobbyId).SendAsync("lobbyPlayerListUpdate", playerList);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }
        #endregion
    }
}