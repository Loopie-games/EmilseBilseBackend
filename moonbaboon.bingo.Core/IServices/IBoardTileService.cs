﻿using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IBoardTileService
    {
        public BoardTile? GetById(string id);
        
        public BoardTile? Create(BoardTile toCreate);
    }
}