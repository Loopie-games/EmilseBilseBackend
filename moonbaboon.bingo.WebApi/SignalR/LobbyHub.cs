using System;
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
    public class LobbyHub: Hub
    {
        private readonly IPendingPlayerService _pendingPlayerService;
        private readonly ILobbyService _lobbyService;

        public LobbyHub(ILobbyService lobbyService, IPendingPlayerService pendingPlayerService)
        {
            _lobbyService = lobbyService;
            _pendingPlayerService = pendingPlayerService;
        }
        
        /// <summary>
        /// Send en Error message to the client
        /// </summary>
        /// <param name="message">Error Message</param>
        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("PopUpError", message);
        }

        private static string GetUserId(HubCallerContext context)
        {
            if (context.User == null) throw new Exception("Could not get user from Context");
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   throw new InvalidOperationException("Could not get userId from Context");
        }
        
        /// <summary>
        /// Adds Authorized user to lobby and sends updates to clients
        /// </summary>
        /// <param name="pin">Pin for lobby</param>
        /// <exception cref="Exception">If User cant be added.</exception>
        public async Task JoinLobby(string pin)
        {
            Console.WriteLine(pin);
            try
            {
                var pp = _lobbyService.JoinLobby(GetUserId(Context), pin);
                await Groups.AddToGroupAsync(Context.ConnectionId, pp.Lobby.Id!);
                await Clients.Caller.SendAsync("receiveLobby", pp.Lobby); /*
                var playerList = _pendingPlayerService.GetByLobbyId(pp.Lobby.Id!).Select(p => new PendingPlayerDto(p))
                    .ToList();
                await Clients.Group(pp.Lobby.Id!).SendAsync("lobbyPlayerListUpdate", playerList);
                */
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }
    }
}