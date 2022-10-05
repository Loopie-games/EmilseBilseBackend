﻿using System;
using System.Collections.Generic;
using System.Linq;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class GameService : IGameService
    {
        private readonly IBoardRepository _boardRepository;
        private readonly IBoardTileRepository _boardTileRepository;
        private readonly IGameRepository _gameRepository;
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IOwnedTilePackRepository _ownedTilePackRepository;
        private readonly IPackTileRepository _packTileRepository;
        private readonly IPendingPlayerRepository _pendingPlayerRepository;

        private readonly Random _random = new();
        private readonly ITilePackRepository _tilePackRepository;
        private readonly ITopPlayerRepository _topPlayerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTileRepository _userTileRepository;

        public GameService(IGameRepository gameRepository, IBoardRepository boardRepository,
            IPendingPlayerRepository pendingPlayerRepository, IUserTileRepository userTileRepository,
            IBoardTileRepository boardTileRepository, IPackTileRepository packTileRepository,
            ITilePackRepository tilePackRepository, ILobbyRepository lobbyRepository, IUserRepository userRepository,
            ITopPlayerRepository topPlayerRepository, IOwnedTilePackRepository ownedTilePackRepository)
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
            _topPlayerRepository = topPlayerRepository;
            _ownedTilePackRepository = ownedTilePackRepository;
        }

        public Game GetById(string id)
        {
            return _gameRepository.FindById(id).Result;
        }

        public string NewGame(string lobbyId, string userId, string[]? tilePackIds)
        {
            //Get lobby and throw exception if not provided with correct host id
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != userId) throw new Exception("only the host of the lobby can start the game");

            var gameId = _gameRepository.Create(new GameEntity(null, userId, null, State.Ongoing)).Result;
            var players = _pendingPlayerRepository.GetByLobbyId(lobbyId).Result;


            if (tilePackIds is null || tilePackIds.Length < 1)
            {
                //Set board and tiles up for each player in lobby
                foreach (var player in players)
                {
                    //Create board for player
                    var board = _boardRepository.Create(player.User.Id, gameId).Result;

                    //get tiles about other players
                    List<BoardTile> boardTilesUser = _userTileRepository.GetTilesForBoard(lobbyId, player.User.Id)
                        .Result
                        .Select((t, i) => new BoardTile(null, board, t, t.User, i, false))
                        .ToList();

                    //if not enough boardtiles for users; fill with default tiles
                    if (boardTilesUser.Count < 24)
                    {
                        List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();

                        var defaultTiles = GetDefaultTiles();
                        //create temp list for refilling in case, that not enough default tiles are available
                        var defaultTilesTemp = new List<PackTile>();

                        while (boardTilesUser.Count < 24)
                        {
                            //Make sure there is enough default tiles
                            if (defaultTilesTemp.Count < 1) defaultTilesTemp = new List<PackTile>(defaultTiles);

                            //find random player
                            var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;

                            //find random default tile
                            var randomTile = defaultTilesTemp[_random.Next(0, defaultTilesTemp.Count)];

                            //Create new board tile
                            boardTilesUser.Add(
                                new BoardTile(null, board, randomTile, rp, boardTilesUser.Count, false));

                            //remove random default tile after used, to avoid duplicate boardtiles
                            defaultTilesTemp.Remove(randomTile);
                        }
                    }

                    //Insert all boardtiles in database
                    var unused = boardTilesUser.Select(boardTile =>
                            _boardTileRepository.Create(new BoardTileEntity(boardTile)).Result)
                        .ToList();
                }
            }
            else
            {
                Console.WriteLine(tilePackIds[0]);

                //Check ownership over chosen packages
                if (tilePackIds.Any(tpId =>
                    !_ownedTilePackRepository.ConfirmOwnership(new OwnedTilePackEntity(userId, tpId)).Result))
                    throw new Exception("You dont have ownership over one or more of the tilepacks");

                var packTiles = new List<PackTile>();

                foreach (var packId in tilePackIds)
                    packTiles.AddRange(_packTileRepository.GetByPackId(packId).Result);

                var boardTilesPack = new List<BoardTile>();
                foreach (var player in players)
                {
                    List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();
                    var board = _boardRepository.Create(player.User.Id, gameId).Result;
                    var boardTilesPlayer = new List<BoardTile>();
                    var packTilesTemp = new List<PackTile>(packTiles);
                    while (boardTilesPlayer.Count < 24 && packTilesTemp.Count > 0)
                    {
                        //find random player
                        var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;

                        var randomTile = packTilesTemp[_random.Next(0, packTilesTemp.Count)];
                        boardTilesPlayer.Add(new BoardTile(null, board, randomTile, rp, boardTilesPlayer.Count,
                            false));
                        packTilesTemp.Remove(randomTile);
                    }

                    boardTilesPack.AddRange(boardTilesPlayer);
                }

                //Insert all boardtiles in database
                var unused = boardTilesPack.Select(boardTile =>
                        _boardTileRepository.Create(new BoardTileEntity(boardTile)).Result)
                    .ToList();
            }

            return gameId;
        }


        /// <exception cref="Exception">if the user is not on the list</exception>
        public List<UserSimple> GetPlayers(string gameId, string userId)
        {
            var players = _gameRepository.GetPlayers(gameId).Result;

            if (players.Any(u => u.Id == userId)) return players;

            throw new Exception("You cannot get player list for a game, that you are not a part of");
        }

        public bool Delete(string gameId, string hostId)
        {
            var game = _gameRepository.FindById(gameId).Result;
            if (game.Host.Id == hostId) return _gameRepository.Delete(gameId).Result;
            throw new Exception("You have to be the host of a game to delete it");
        }

        public Game ConfirmWin(string gameId, string hostId)
        {
            var game = _gameRepository.FindById(gameId).Result;
            if (game.Host.Id != hostId) throw new Exception("Only the host can Confirm a win");

            game.State = State.Ended;
            var topRanked = _boardRepository.FindTopRanking(gameId, 3).Result;
            foreach (var board in topRanked)
            {
                var user = _userRepository.ReadById(board.UserId).Result;
                var topPlayer = _topPlayerRepository.Create(new TopPlayer(null, gameId, user, board.TurnedTiles))
                    .Result;
            }

            return _gameRepository.Update(game).Result;
        }

        public Game PauseGame(Game game, string userId)
        {
            if (!_gameRepository.GetPlayers(game.Id).Result.Any(u => u.Id == userId))
                throw new Exception("You cant pause games that you are not apart of");


            game.State = State.Paused;
            game.Winner = _userRepository.ReadById(userId).Result;

            return _gameRepository.Update(game).Result;
        }

        public Game DenyWin(string gameId, string userId)
        {
            var game = _gameRepository.FindById(gameId).Result;

            if (game.Host.Id != userId) throw new Exception("Only the host can deny wins");

            game.State = State.Ongoing;
            game.Winner = null;

            return _gameRepository.Update(game).Result;
        }

        private List<PackTile> GetDefaultTiles()
        {
            return _packTileRepository
                .GetByPackId(_tilePackRepository.FindDefault().Result.Id ??
                             throw new InvalidOperationException("No default Tilepack"))
                .Result;
        }
    }
}