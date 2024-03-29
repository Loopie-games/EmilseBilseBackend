﻿using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IAuthRepository
    {
        public Task<string> Create(AuthEntity entity);
    }
}