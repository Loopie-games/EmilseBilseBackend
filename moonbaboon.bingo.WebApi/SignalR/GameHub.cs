using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

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
            var lobby = _lobbyService.Create(new Lobby(hostId));
            if (lobby?.Pin != null)
            {
                var pp = _lobbyService.JoinLobby(hostId, lobby.Pin);
            }

            if (lobby?.Id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Caller.SendAsync("receiveLobby", lobby);
            }
            
        }
        
        

    }
}