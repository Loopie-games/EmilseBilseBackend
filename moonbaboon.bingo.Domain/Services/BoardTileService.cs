using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BoardTileService : IBoardTileService
    {
        private readonly IBoardTileRepository _boardTileRepository;
        private readonly IGameRepository _gameRepository;

        public BoardTileService(IBoardTileRepository boardTileRepository, IGameRepository gameRepository)
        {
            _boardTileRepository = boardTileRepository;
            _gameRepository = gameRepository;
        }

        public BoardTile GetById(string id)
        {
            return _boardTileRepository.ReadById(id).Result;
        }

        public BoardTileEntity Create(BoardTileEntity toCreate)
        {
            return _boardTileRepository.Create(toCreate).Result;
        }

        public List<BoardTile> GetByBoardId(string id)
        {
            return _boardTileRepository.FindByBoardId(id).Result;
        }

        public BoardTileEntity TurnTile(string boardTileId, string userId)
        {
            try
            {
                //checks that the user trying to turn the tile is owner of the board, else Error
                var boardTile = _boardTileRepository.ReadById(boardTileId).Result;
                if (boardTile.Board.UserId != userId)
                    throw new Exception("You do not own this board, and can not turn the tiles!");

                var game = _gameRepository.FindById(boardTile.Board.GameId).Result;
                if (game.State != State.Ongoing)
                    throw new Exception("You cannot turn tiles when game is " + Enum.GetName(game.State));

                boardTile.IsActivated = !boardTile.IsActivated;
                var tile = _boardTileRepository.Update(new BoardTileEntity(boardTile)).Result;
                return tile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}