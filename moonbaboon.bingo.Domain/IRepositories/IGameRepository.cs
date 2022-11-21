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
        public Task<string> Create(GameEntity toCreate);

        /// <summary>
        /// Deletes game from the database
        /// </summary>
        /// <param name="gameId">id of the game</param>
        /// <returns>bool representing the successful execution of the task</returns>
        public Task Delete(string gameId);

        /// <summary>
        /// Update game entry in the database
        /// </summary>
        /// <param name="game">game to update</param>
        /// <returns>the updated game</returns>
        public Task Update(Game game);
    }
}