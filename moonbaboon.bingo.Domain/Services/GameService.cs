using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class GameService: IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILobbyRepository _lobbyRepository;

        public GameService(IGameRepository gameRepository, ILobbyRepository lobbyRepository)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
        }

        public Game? GetById(string id)
        {
            return _gameRepository.FindById(id).Result;
        }

        public Game? Create(string lobbyId, string hostId)
        {
            var lobby = _lobbyRepository.FindById(lobbyId).Result;

            if (lobby?.Host == hostId)
            {
                return _gameRepository.Create(hostId).Result;
            }
            return  null;
        }
    }
}