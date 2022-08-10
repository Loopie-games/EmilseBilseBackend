using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;

        public LobbyService(ILobbyRepository lobbyRepository)
        {
            _lobbyRepository = lobbyRepository;
        }

        public Lobby? GetById(string id)
        {
            return _lobbyRepository.FindById(id).Result;
        }

        public Lobby? Create(Lobby lobbyToCreate)
        {
            return _lobbyRepository.Create(lobbyToCreate).Result;
        }
    }
}