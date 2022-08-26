﻿using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITilePackRepository
    {
        public Task<List<TilePack>> FindAll();
    }
}