﻿using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardTileRepository
    {
        public Task<BoardTile?> FindById(string id);

        public Task<BoardTile?> Create(BoardTile toCreate);

    }
}