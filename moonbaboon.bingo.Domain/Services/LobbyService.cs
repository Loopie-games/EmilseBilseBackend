﻿using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;
        private readonly IUserRepository _userRepository;

        public LobbyService(ILobbyRepository lobbyRepository, IPendingPlayerRepository pendingPlayerRepository, IUserRepository userRepository)
        {
            _lobbyRepository = lobbyRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _userRepository = userRepository;
        }

        public LobbyForUser? GetById(string id)
        {
            return _lobbyRepository.FindById_ForUser(id).Result;
        }

        public Lobby? FindByPin(string pin)
        {
            return _lobbyRepository.FindByPin(pin).Result;
        }

        public Lobby? Create(Lobby lobbyToCreate)
        {
            return _lobbyRepository.Create(lobbyToCreate).Result;
        }

        public PendingPlayer? JoinLobby(string userId, string pin)
        {
            var user = _userRepository.ReadById(userId).Result;
            var lobby = _lobbyRepository.FindByPin(pin).Result;
            if (lobby?.Id == null || user?.Id == null)
            {
                return null;
            }
            var pp = _pendingPlayerRepository.IsPlayerInLobby(userId, lobby.Id).Result;
            if (pp != null)
            {
                return pp;
            }
            return _pendingPlayerRepository.Create(new PendingPlayer(new UserSimple(user), lobby)).Result;
        }
    }
}