﻿using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ILobbyRepository
    {
        public Task<Lobby?> Create(Lobby lobbyToCreate);
        
        public Task<LobbyForUser?> FindById_ForUser(string id);
        
        public Task<Lobby?> FindById(string id);
        
        public Task<Lobby?> FindByPin(string pin);
    }
}