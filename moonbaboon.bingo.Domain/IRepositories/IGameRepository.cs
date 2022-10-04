using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IGameRepository
    {
        /// <summary>
        /// Find a game object from an Id
        /// </summary>
        /// <param name="id"> Id string</param>
        /// <returns>Game with the given id</returns>
        public Task<Game> FindById(string id);

        /// <summary>
        /// Inserts a new game in database
        /// </summary>
        /// <param name="hostId">host of the lobby</param>
        /// <returns>Game</returns>
        public Task<Game> Create(string hostId);

        /// <summary>
        /// Gets list of players is in game with given Id
        /// </summary>
        /// <param name="gameId">Id specific for game</param>
        /// <returns>Player list</returns>
        public Task<List<UserSimple>> GetPlayers(string gameId);

        /// <summary>
        /// Deletes game from the database
        /// </summary>
        /// <param name="gameId">id of the game</param>
        /// <returns>bool representing the successful execution of the task</returns>
        public Task<bool> Delete(string gameId);

        /// <summary>
        /// Update game entry in the database
        /// </summary>
        /// <param name="game">game to update</param>
        /// <returns>the updated game</returns>
        public Task<Game> Update(Game game);
        public Task<List<Game>> GetEnded(string userId);
    }
}