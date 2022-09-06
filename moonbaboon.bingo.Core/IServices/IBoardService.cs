using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBoardService
    {
        public Board? GetById(string id);

        public Board? GetByUserAndGameId(string userId, string gameId);
        public Board? CreateBoard(string userId, string gameId);
        public bool IsBoardFilled(string? boardId);
    }
}