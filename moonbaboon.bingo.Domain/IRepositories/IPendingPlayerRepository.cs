using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IPendingPlayerRepository
    {
        /// <summary>
        ///     Creates PendingPlayer in Database
        /// </summary>
        /// <param name="entity"> PendingPlayer to insert in the database</param>
        /// <returns></returns>
        public Task<string> Create(PendingPlayer entity);

        public Task<PendingPlayer> GetByUserId(string userId);

        public Task<List<PendingPlayer>> GetByLobbyId(string lobbyId);

        /// <summary>
        ///     Checks if PendingPlayer with given id exist in database
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Pending player if such exists with given ids, else null</returns>
        public Task<PendingPlayer?> IsPlayerInLobby(string userId);

        public Task DeleteWithLobbyId(string lobbyId);
        public Task Delete(string Id);
        public Task Update(PendingPlayer entity);
    }
}