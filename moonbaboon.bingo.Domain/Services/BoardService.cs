using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;

        public BoardService(IBoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }

        public BoardEntity GetById(string id)
        {
            return _boardRepository.FindById(id).Result;
        }

        public BoardEntity? GetByUserAndGameId(string userId, string gameId)
        {
            
                return _boardRepository.FindByUserAndGameId(userId, gameId).Result;
            
            
        }

        public bool IsBoardFilled(string? boardId)
        {
            return _boardRepository.IsBoardFilled(boardId).Result;
        }
    }
}