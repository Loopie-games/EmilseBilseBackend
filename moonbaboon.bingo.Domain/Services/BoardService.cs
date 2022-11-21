using System;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BoardService : IBoardService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IGameRepository _gameRepository;

        public BoardService(IBoardRepository boardRepository, IGameRepository gameRepository)
        {
            _boardRepository = boardRepository;
            _gameRepository = gameRepository;
        }

        public BoardEntity GetById(string id)
        {
            return _boardRepository.FindById(id).Result;
        }

        public BoardEntity? GetByUserAndGameId(string userId, string gameId)
        {
            try
            {
                return _boardRepository.FindByUserAndGameId(userId, gameId).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public BoardEntity? CreateBoard(string userId, string gameId)
        {
            var game = _gameRepository.FindById(gameId).Result;

            return game.Winner is not null ? null : _boardRepository.Create(new BoardEntity(null, gameId, userId)).Result;
        }

        public bool IsBoardFilled(string? boardId)
        {
            return _boardRepository.IsBoardFilled(boardId).Result;
        }
    }
}