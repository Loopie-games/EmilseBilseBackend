using System;
using moonbaboon.bingo.Core.IServices;
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

        public Lobby? GetById(string id)
        {
            return _lobbyRepository.FindById(id).Result;
        }

        public Lobby? GetByHostId(string hostId)
        {
            return _lobbyRepository.FindByHostId(hostId).Result;
        }

        public Lobby? FindByPin(string pin)
        {
            return _lobbyRepository.FindByPin(pin).Result;
        }

        public Lobby? Create(Lobby lobbyToCreate)
        {
            var lobby = _lobbyRepository.Create(lobbyToCreate).Result;
            if (lobby?.Pin != null)
            {
                var pp = JoinLobby(lobby.Host, lobby.Pin);
            }
            return lobby;
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

        public bool CloseLobby(string lobbyId, string hostId)
        {
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby?.Host == hostId)
            {
                if (_pendingPlayerRepository.DeleteWithLobbyId(lobbyId).Result)
                {
                    return _lobbyRepository.DeleteLobby(lobbyId).Result;
                }
            }
            return false;
        }

        public bool LeaveLobby(string lobbyId, string userId)
        {
            var pp = _pendingPlayerRepository.IsPlayerInLobby(userId, lobbyId).Result;
            if (pp != null)
            {
                if(_lobbyRepository.FindById(lobbyId).Result?.Host == userId)
                {
                    return CloseLobby(lobbyId, userId);
                }
                return _pendingPlayerRepository.Delete(pp.Id).Result;
            }

            return false;
        }
    }
}