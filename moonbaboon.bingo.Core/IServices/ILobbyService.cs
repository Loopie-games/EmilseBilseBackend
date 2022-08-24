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
        /// <summary>
        /// Adds the User to the Lobby corresponding to the given pin
        /// </summary>
        /// <param name="userId">The Users Id</param>
        /// <param name="pin">The Pin to access a specific Lobby</param>
        /// <returns>A PendingPlayer containing the Lobby and User</returns>
        public PendingPlayer JoinLobby(string userId, string pin);

        public bool CloseLobby(string lobbyId, string hostId);

        public bool LeaveLobby(string lobbyId, string userId);

    }
}