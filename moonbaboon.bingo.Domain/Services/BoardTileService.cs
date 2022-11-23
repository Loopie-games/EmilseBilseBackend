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
        private readonly IBoardRepository _boardRepository;

        public BoardTileService(IBoardTileRepository boardTileRepository, IGameRepository gameRepository, IBoardRepository boardRepository)
        {
            _boardTileRepository = boardTileRepository;
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
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
            //checks that the user trying to turn the tile is owner of the board, else Error
            var boardTile = _boardTileRepository.ReadById(boardTileId).Result;
            var board = _boardRepository.FindByUserAndGameId2(userId, boardTile.BoardEntity.GameId);
            if (board is null)
                throw new Exception("You are not a member of this board, and can not turn the tiles!");

            var game = _gameRepository.FindById(boardTile.BoardEntity.GameId).Result;
            if (game.State != State.Ongoing)
                throw new Exception("You cannot turn tiles when game is " + Enum.GetName(game.State));

            var bt = new BoardTileEntity(boardTile)
            {
                ActivatedBy = userId
            };
            var tile = _boardTileRepository.Update(bt).Result;
            return tile;
        }
    }
}