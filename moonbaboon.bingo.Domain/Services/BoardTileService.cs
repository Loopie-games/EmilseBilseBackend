using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class BoardTileService: IBoardTileService
    {
        private readonly IBoardTileRepository _boardTileRepository;

        public BoardTileService(IBoardTileRepository boardTileRepository)
        {
            _boardTileRepository = boardTileRepository;
        }

        public BoardTile GetById(string id)
        {
            return _boardTileRepository.FindById(id).Result;
        }

        public BoardTile Create(BoardTile toCreate)
        {
            return _boardTileRepository.Create(toCreate).Result;
        }

        public List<BoardTile> GetByBoardId(string id)
        {
            return _boardTileRepository.FindByBoardId(id).Result;
        }

        public BoardTile TurnTile(string boardTileId, string userId)
        {
            var boardTile = _boardTileRepository.FindById(boardTileId).Result;
            boardTile.IsActivated = !boardTile.IsActivated;
            return _boardTileRepository.Update(boardTile).Result;
        }
    }
}