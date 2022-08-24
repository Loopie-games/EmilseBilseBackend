using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ILobbyRepository
    {
        public Task<Lobby?> Create(Lobby lobbyToCreate);
        
        public Task<LobbyForUser?> FindById_ForUser(string id);
        
        public Task<Lobby?> FindById(string id);
        
        public Task<Lobby?> FindByHostId(string hostId);
        /// <summary>
        /// Finds the Lobby corresponding to the given Pin
        /// </summary>
        /// <param name="pin">Specific pin for lobby</param>
        /// <returns>Task with the Lobby as Result</returns>
        public Task<Lobby> FindByPin(string pin);
        public Task<bool> DeleteLobby(string lobbyId);
        
    }
}