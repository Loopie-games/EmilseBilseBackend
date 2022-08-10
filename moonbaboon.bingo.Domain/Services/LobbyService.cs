using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;

        public LobbyService(ILobbyRepository lobbyRepository, IPendingPlayerRepository pendingPlayerRepository)
        {
            _lobbyRepository = lobbyRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
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
            var lobby = _lobbyRepository.FindByPin(pin).Result;
            if (lobby == null)
            {
                return null;
            }
            return _pendingPlayerRepository.Create(new PendingPlayer(userId, lobby)).Result;
        }
    }
}