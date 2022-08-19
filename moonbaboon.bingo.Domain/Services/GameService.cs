using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IBoardTileRepository _boardTileRepository;

        public GameService(IGameRepository gameRepository, ILobbyRepository lobbyRepository, IBoardRepository boardRepository, IPendingPlayerRepository pendingPlayerRepository, ITileRepository tileRepository, IBoardTileRepository boardTileRepository)
        {
            _gameRepository = gameRepository;
            _lobbyRepository = lobbyRepository;
            _boardRepository = boardRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _tileRepository = tileRepository;
            _boardTileRepository = boardTileRepository;
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
            var random = new Random();
            var game = _gameRepository.Create(lobby.Host).Result;
            //todo throw Error "Game not created"
            if (game == null) return game;
            
            var players = _pendingPlayerRepository.GetByLobbyId(lobby.Id).Result;
            
            foreach (var player in players)
            {
                var board = _boardRepository.Create(player.User.Id, game.Id).Result;
                
                //todo throw error
                if (board == null) continue;
                List<Tile> usableTiles = _tileRepository.GetTilesForBoard(players, player.User.Id).Result;

                if (usableTiles.Count < 24)
                {
                    List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();
                    var t =_tileRepository.Create(usablePlayers[random.Next(0, usablePlayers.Count - 1)].User.Id,
                        "filler", player.User.Id).Result;
                    
                    while (usableTiles.Count < 24)
                    {
                        usableTiles.Add(t);
                    }
                }
                for (int i = 0; i < 24; i++)
                {
                    var tile = usableTiles[random.Next(0, usableTiles.Count - 1)];
                    var boardtile = _boardTileRepository.Create(new BoardTile(board,tile.Id!,i, false)).Result;
                    usableTiles.Remove(tile);
                }
            }

            return game;
        }
    }
}