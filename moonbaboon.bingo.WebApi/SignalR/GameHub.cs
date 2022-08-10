using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.Services;

namespace moonbaboon.bingo.WebApi.SignalR
{
    public class GameHub : Hub
    {
        private readonly ILobbyService _lobbyService;

        public GameHub(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        public async Task CreateLobby(string hostId)
        {
            var lobby = _lobbyService.Create(new Lobby(hostId));
            if (lobby?.Id != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Caller.SendAsync("receiveLobby", lobby);
            }


        }

    }
}