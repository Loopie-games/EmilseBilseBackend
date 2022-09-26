﻿using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class PendingPlayerService : IPendingPlayerService
    {
        private readonly IPendingPlayerRepository _pendingPlayerRepository;

        public PendingPlayerService(IPendingPlayerRepository pendingPlayerRepository)
        {
            _pendingPlayerRepository = pendingPlayerRepository;
        }

        public List<PendingPlayer> GetByLobbyId(string lobbyId)
        {
            return _pendingPlayerRepository.GetByLobbyId(lobbyId).Result;
        }

        public PendingPlayer GetByUserId(string userId)
        {
            return _pendingPlayerRepository.GetByUserId(userId).Result;
        }
    }
}