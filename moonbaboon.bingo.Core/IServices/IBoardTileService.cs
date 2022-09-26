﻿using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBoardTileService
    {
        public BoardTile GetById(string id);

        public BoardTile? Create(BoardTile toCreate);
        public List<BoardTile> GetByBoardId(string id);

        /// <summary>
        ///     (De)Activates given tile
        /// </summary>
        /// <param name="boardTileId">id of the tile</param>
        /// <param name="userId">id of the user turning the tile</param>
        /// <returns>the turned boardtile</returns>
        public BoardTile TurnTile(string boardTileId, string userId);
    }
}