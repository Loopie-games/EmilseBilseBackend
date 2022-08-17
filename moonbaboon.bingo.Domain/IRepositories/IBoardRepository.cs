﻿using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IBoardRepository
    {
        public Task<Board?> FindById(string id);
        public Task<Board?> Create(string userId, string gameId);
    }
}