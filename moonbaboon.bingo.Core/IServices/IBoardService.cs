using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBoardService
    {
        public Board GetById(string id);

        /// <summary>
        ///     Get the board for a player of a game, if such exists
        /// </summary>
        /// <param name="userId">UserId for the player</param>
        /// <param name="gameId">Id for the game</param>
        /// <returns>Board for the player</returns>
        public BoardEntity? GetByUserAndGameId(string userId, string gameId);

        public bool IsBoardFilled(string boardId);
    }
}