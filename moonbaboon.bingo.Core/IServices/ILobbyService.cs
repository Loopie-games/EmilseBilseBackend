using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ILobbyService
    {
        public Lobby? GetById(string id);
        public Lobby? GetByHostId(string hostId);
        public Lobby? FindByPin(string pin);
        
        public Lobby? Create(Lobby lobbyToCreate);
        
        public PendingPlayer JoinLobby(string userId, string pin);

        public bool CloseLobby(string lobbyId, string hostId);

        public bool LeaveLobby(string lobbyId, string userId);

    }
}