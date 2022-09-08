using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardRepository
    {
        public Task<Board> FindById(string id);
        public Task<Board> Create(string userId, string gameId);
        /// <summary>
        /// Searches database for a board corresponding to the given game and player
        /// </summary>
        /// <param name="userId">userId for the player</param>
        /// <param name="gameId">id for the game</param>
        /// <returns>the board if it exist, else null</returns>
        public Task<Board?> FindByUserAndGameId(string userId, string gameId);
        public Task<bool> IsBoardFilled(string boardId);
    }
}