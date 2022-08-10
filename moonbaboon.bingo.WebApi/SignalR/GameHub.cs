using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.SignalR
{
    public class GameHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("onConnect", "Connected");
            
            return base.OnConnectedAsync();
        }

        public async Task CreateLobby(string hostId)
        {
            
        }

    }
}