using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IGameRepository
    {
        public Task<Game> FindById(string id);

        public Task<Game?> FindByHostId(string userId);

        /// <summary>
        ///     Inserts a new game in database
        /// </summary>
        /// <param name="hostId">host of the lobby</param>
        /// <returns>Game</returns>
        public Task<Game> Create(string hostId);

        /// <summary>
        ///     Gets list of players is in game with given Id
        /// </summary>
        /// <param name="gameId">Id specific for game</param>
        /// <returns>Player list</returns>
        public Task<List<UserSimple>> GetPlayers(string gameId);

        public Task<bool> Delete(string gameId);
        public Task<Game> Update(Game game);
        public Task<List<Game>> GetEnded(string userId);
    }
}