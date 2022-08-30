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
        private readonly IUserTileRepository _userTileRepository;
        private readonly IBoardTileRepository _boardTileRepository;
        private readonly IPackTileRepository _packTileRepository;
        private readonly ITilePackRepository _tilePackRepository;
        
        private readonly Random _random = new();

        public GameService(IGameRepository gameRepository, IBoardRepository boardRepository, IPendingPlayerRepository pendingPlayerRepository, IUserTileRepository userTileRepository, IBoardTileRepository boardTileRepository, IPackTileRepository packTileRepository, ITilePackRepository tilePackRepository)
        {
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _userTileRepository = userTileRepository;
            _boardTileRepository = boardTileRepository;
            _packTileRepository = packTileRepository;
            _tilePackRepository = tilePackRepository;
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
            try
            {
                var game = _gameRepository.Create(lobby.Host).Result;
            if (game == null) throw new Exception("Game wasn't created");
            
            var players = 
                _pendingPlayerRepository.GetByLobbyId(lobby.Id ?? 
                                                      throw new InvalidOperationException("Cannot Create game with invalid Lobby")).Result;
            
            foreach (var player in players)
            {
                try
                {
                    var board = _boardRepository.Create(
                        player.User.Id ??
                        throw new InvalidOperationException("Cannot create board for user with null ID"),
                        game.Id ?? throw new InvalidOperationException("Cannot create board for game with null ID")
                    ).Result;

                    if (board == null)
                        throw new Exception("Board wasn't created for player with username " + player.User.Username);
                    List<BoardTile> boardTiles = new();
                    List<UserTile> userTiles = new (_userTileRepository.GetTilesForBoard(lobby.Id, player.User.Id).Result);
                    var index = 0;
                    foreach (var tile in userTiles)
                    {
                        boardTiles.Add(new BoardTile(null, board, tile, tile.User, index, false));
                        index++;
                    }
                    if (boardTiles.Count < 24)
                    {
                        List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();
                        var defaultTiles = _packTileRepository.GetByPackId(_tilePackRepository.FindDefault().Result.Id ?? throw new InvalidOperationException("No default Tilepack"))
                            .Result;
                        var defaultTilesTemp = new List<PackTile>(defaultTiles);

                        while (boardTiles.Count < 24)
                        {
                            var defaultTile = defaultTilesTemp[_random.Next(0, defaultTilesTemp.Count)];
                            var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;
                            boardTiles.Add(new BoardTile(null, board,defaultTile, rp,index,false));
                            index++;
                            defaultTilesTemp.Remove(defaultTile);
                            if (defaultTilesTemp.Count <1)
                            { 
                                defaultTilesTemp = new List<PackTile>(defaultTiles);
                            }
                        }
                    }
                    foreach (var boardTile in boardTiles)
                    {
                        var bt = _boardTileRepository.Create(boardTile).Result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return game;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<UserSimple> GetPlayers(string gameId)
        {
            return _gameRepository.GetPlayers(gameId).Result;
        }
    }
}