using System;
using System.Collections.Generic;
using System.Linq;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;
        private readonly IUserTileRepository _userTileRepository;
        private readonly IBoardTileRepository _boardTileRepository;
        private readonly IPackTileRepository _packTileRepository;
        private readonly ITilePackRepository _tilePackRepository;
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IUserRepository _userRepository;

        private readonly Random _random = new();

        public GameService(IGameRepository gameRepository, IBoardRepository boardRepository,
            IPendingPlayerRepository pendingPlayerRepository, IUserTileRepository userTileRepository,
            IBoardTileRepository boardTileRepository, IPackTileRepository packTileRepository,
            ITilePackRepository tilePackRepository, ILobbyRepository lobbyRepository, IUserRepository userRepository)
        {
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
            _pendingPlayerRepository = pendingPlayerRepository;
            _userTileRepository = userTileRepository;
            _boardTileRepository = boardTileRepository;
            _packTileRepository = packTileRepository;
            _tilePackRepository = tilePackRepository;
            _lobbyRepository = lobbyRepository;
            _userRepository = userRepository;
        }

        public Game GetById(string id)
        {
            return _gameRepository.FindById(id).Result;
        }

        public Game Create(string hostId)
        {
            return _gameRepository.Create(hostId).Result;
        }

        public Game NewGame(string lobbyId, string userId)
        {
            try
            {
                //if user is already host for a game, it is deleted
                var oldGame = _gameRepository.FindByHostId(userId).Result;
                if (oldGame?.Id is not null)
                {
                    _gameRepository.Delete(oldGame.Id);
                }

                //Get lobby and throw exception if not provided with correct host id
                var lobby = _lobbyRepository.FindById(lobbyId).Result;
                if (lobby.Host != userId)
                {
                    throw new Exception("only the host of the lobby can start the game");
                }

                var game = _gameRepository.Create(userId).Result;
                var players = _pendingPlayerRepository.GetByLobbyId(lobbyId).Result;

                //Set board and tiles up for each player in lobby
                foreach (var player in players)
                {
                    //Create board for player
                    var board = _boardRepository.Create(player.User.Id, game.Id).Result;

                    //get tiles about other players
                    List<BoardTile> boardTiles = _userTileRepository.GetTilesForBoard(lobbyId, player.User.Id)
                        .Result
                        .Select((t, i) => new BoardTile(null, board, t, t.User, i, false))
                        .ToList();

                    //if not enough boardtiles for users; fill with default tiles
                    if (boardTiles.Count < 24)
                    {
                        List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();

                        var defaultTiles = GetDefaultTiles();
                        //create temp list for refilling in case, that not enough default tiles are available
                        var defaultTilesTemp = new List<PackTile>();

                        while (boardTiles.Count < 24)
                        {
                            //Make sure there is enough default tiles
                            if (defaultTilesTemp.Count < 1)
                            {
                                defaultTilesTemp = new List<PackTile>(defaultTiles);
                            }

                            //find random player
                            var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;

                            //find random default tile
                            var randomTile = defaultTilesTemp[_random.Next(0, defaultTilesTemp.Count)];

                            //Create new board tile
                            boardTiles.Add(
                                new BoardTile(null, board, randomTile, rp, boardTiles.Count, false));

                            //remove random default tile after used, to avoid duplicate boardtiles
                            defaultTilesTemp.Remove(randomTile);
                        }
                    }

                    //Insert all boardtiles in database
                    var unused = boardTiles.Select(boardTile => _boardTileRepository.Create(boardTile).Result)
                        .ToList();
                }

                return game;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<PackTile> GetDefaultTiles()
        {
            return _packTileRepository
                .GetByPackId(_tilePackRepository.FindDefault().Result.Id ??
                             throw new InvalidOperationException("No default Tilepack"))
                .Result;
        }


        /// <exception cref="Exception">if the user is not on the list</exception>
        public List<UserSimple> GetPlayers(string gameId, string userId)
        {
            var players = _gameRepository.GetPlayers(gameId).Result;

            if (players.Any(u => u.Id == userId))
            {
                return players;
            }

            throw new Exception("You cannot get player list for a game, that you are not a part of");
        }

        public bool Delete(string gameId, string hostId)
        {
            try
            {
                var game = _gameRepository.FindById(gameId).Result;

                if (game.Host.Id == hostId)
                {
                    return _gameRepository.Delete(gameId).Result;
                }

                throw new Exception("You have to be the host of a game to delete it");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Game ConfirmWin(string gameId, string hostId)
        {
            try
            {
                var game = _gameRepository.FindById(gameId).Result;
                if (game.Host.Id != hostId)
                {
                    throw new Exception("Only the host can Confirm a win");
                }
                game.State = State.Ended;
                return _gameRepository.Update(game).Result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Game PauseGame(Game game, string userId)
        {
            try
            {
                if (!_gameRepository.GetPlayers(game.Id).Result.Any(u => u.Id == userId))
                {
                    throw new Exception("You cant pause games that you are not apart of");
                }
                
                
                game.State = State.Paused;
                game.Winner = new UserSimple(_userRepository.ReadById(userId).Result);

                return _gameRepository.Update(game).Result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Game DenyWin(string gameId, string userId)
        {
            try
            {
                var game = _gameRepository.FindById(gameId).Result;
                
                if (game.Host.Id != userId)
                {
                    throw new Exception("Only the host can deny wins");
                }
                game.State = State.Ongoing;
                game.Winner = null;

                return _gameRepository.Update(game).Result;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}