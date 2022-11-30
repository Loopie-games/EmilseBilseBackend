using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardRepository
    {
        public Task<Board> FindById(string id);
        public Task<string> Create(BoardEntity entity);

        BoardEntity FindByUserAndGameId(string userId, string gameId);

        public Task<bool> IsBoardFilled(string boardId);

        public Task<List<BoardEntity>> FindTopRanking(string gameId, int limit);
        
    }
}