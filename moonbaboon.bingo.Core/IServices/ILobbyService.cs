using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ILobbyService
    {
        public Lobby? GetById(string id);
        
        /// <summary>
        /// Gets Lobby corresponding to the given hostId, if such exists
        /// </summary>
        /// <param name="hostId">UserId for host of the Lobby</param>
        /// <returns>Lobby with given hostId if such exists, else null</returns>
        public Lobby? GetByHostId(string hostId);

        public Lobby? Create(string hostId);
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