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
        private readonly IBoardRepository _boardRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;
        private readonly ITileRepository _tileRepository;
        private readonly IBoardTileRepository _boardTileRepository;
        
        private readonly Random _random = new();

        public GameService(IGameRepository gameRepository, IBoardRepository boardRepository, IPendingPlayerRepository pendingPlayerRepository, ITileRepository tileRepository, IBoardTileRepository boardTileRepository)
        {
            _gameRepository = gameRepository;
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

        
        //Todo description
        public Game NewGame(Lobby lobby)
        {
            var game = _gameRepository.Create(lobby.Host).Result;
            if (game == null) throw new Exception("Game wasn't created");
            
            var players = 
                _pendingPlayerRepository.GetByLobbyId(lobby.Id ?? 
                                                      throw new InvalidOperationException("Cannot Create game with invalid Lobby")).Result;
            
            foreach (var player in players)
            {
                var board = _boardRepository.Create(
                    player.User.Id ?? throw new InvalidOperationException("Cannot create board for user with null ID"), 
                    game.Id ?? throw new InvalidOperationException("Cannot create board for game with null ID")
                    ).Result;
                
                if (board == null) throw new Exception("Board wasn't created for player with username " + player.User.Username);
                
                List<Tile> usableTiles = _tileRepository.GetTilesForBoard(lobby.Id, player.User.Id).Result;
                if (usableTiles.Count < 24)
                {
                    List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();

                    var i = (24 - usableTiles.Count) / usablePlayers.Count;
                        Tile? filler = null;
                        foreach (var pp in usablePlayers)
                        {
                            filler = _tileRepository.FindFiller(pp.User.Id!).Result ?? _tileRepository.Create(pp.User.Id!,
                                "filler", player.User.Id).Result;
                            for (var j = 0; j < i; j++)
                            {
                               usableTiles.Add(filler!); 
                            }
                        }
                        while (usableTiles.Count < 24)
                        {
                            usableTiles.Add(filler!);
                        }
                        
                }
                for (var i = 0; i < 24; i++)
                {
                    var tile = usableTiles[_random.Next(0, usableTiles.Count - 1)];
                    var boardTile = _boardTileRepository.Create(new BoardTile(board,tile.Id!,i, false)).Result;
                    usableTiles.Remove(tile);
                }
            }

            return game;
        }

        public List<UserSimple> GetPlayers(string gameId)
        {
            return _gameRepository.GetPlayers(gameId).Result;
        }
    }
}