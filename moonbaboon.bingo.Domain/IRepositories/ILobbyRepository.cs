using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ILobbyRepository
    {
        public Task<string> Create(Lobby entity);

        public Task<Lobby> FindById(string id);

        /// <summary>
        ///     Finds the Lobby corresponding to the given HostId
        /// </summary>
        /// <param name="hostId">User Id for the host of the Lobby</param>
        /// <returns>Task with the Lobby as Result</returns>
        public Task<Lobby?> FindByHostId(string hostId);

        /// <summary>
        ///     Finds the Lobby corresponding to the given Pin
        /// </summary>
        /// <param name="pin">Specific pin for lobby</param>
        /// <returns>Task with the Lobby as Result</returns>
        public Task<Lobby> FindByPin(string pin);

        public Task DeleteLobby(string lobbyId);
    }
}