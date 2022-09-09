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

        public BoardTileService(IBoardTileRepository boardTileRepository)
        {
            _boardTileRepository = boardTileRepository;
        }

        public BoardTile GetById(string id)
        {
            return _boardTileRepository.ReadById(id).Result;
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
            try
            {
                //checks that the user trying to turn the tile is owner of the board, else Error
                var boardTile = _boardTileRepository.ReadById(boardTileId).Result;
                if (boardTile.Board.UserId != userId)
                {
                    throw new Exception("You do not own this board, and can not turn the tiles!");
                }
                
                boardTile.IsActivated = !boardTile.IsActivated;
                var tile = _boardTileRepository.Update(boardTile).Result;
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