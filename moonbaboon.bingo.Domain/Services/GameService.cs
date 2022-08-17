using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class GameService: IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;
        private readonly ITileRepository _tileRepository;

        public GameService(IGameRepository gameRepository, ILobbyRepository lobbyRepository, IBoardRepository boardRepository, IPendingPlayerRepository pendingPlayerRepository, ITileRepository tileRepository)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
            _boardRepository = boardRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _tileRepository = tileRepository;
        }

        public Game? GetById(string id)
        {
            return _gameRepository.FindById(id).Result;
        }

        public Game? Create(string hostId)
        {
            return _gameRepository.Create(hostId).Result;
        }

        public Game? NewGame(Lobby lobby)
        {
            var game = _gameRepository.Create(lobby.Host).Result;
            if (game != null)
            {
                var players = _pendingPlayerRepository.GetByLobbyId(lobby.Id).Result;
                var tiles = new List<Tile>();
                foreach (var player in players)
                {
                    
                }
            }
            
            return game;
        }
    }
}