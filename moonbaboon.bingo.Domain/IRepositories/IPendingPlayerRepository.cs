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
        /// <param name="toCreate"> PendingPlayer to insert in the database</param>
        /// <returns></returns>
        public Task<PendingPlayer> Create(PendingPlayer toCreate);

        public Task<PendingPlayer> GetByUserId(string userId);

        public Task<List<PendingPlayer>> GetByLobbyId(string lobbyId);

        /// <summary>
        ///     Checks if PendingPlayer with given id exist in database
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Pending player if such exists with given ids, else null</returns>
        public Task<PendingPlayer?> IsPlayerInLobby(string userId);

        public Task<bool> DeleteWithLobbyId(string lobbyId);
        public Task<bool> Delete(string? ppId);
        public Task<PendingPlayer> Update(PendingPlayer toUpdate);
    }
}