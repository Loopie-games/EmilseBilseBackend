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
        private readonly IBoardMemberRepository _boardMemberRepository;
        private readonly Random _random = new();
        private readonly ITilePackRepository _tilePackRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTileRepository _userTileRepository;

        public GameService(IGameRepository gameRepository, IBoardRepository boardRepository,
            IPendingPlayerRepository pendingPlayerRepository, IUserTileRepository userTileRepository,
            IBoardTileRepository boardTileRepository, IPackTileRepository packTileRepository,
            ITilePackRepository tilePackRepository, ILobbyRepository lobbyRepository, IUserRepository userRepository, 
            IOwnedTilePackRepository ownedTilePackRepository, IBoardMemberRepository boardMemberRepository)
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
            _ownedTilePackRepository = ownedTilePackRepository;
            _boardMemberRepository = boardMemberRepository;
        }

        public Game GetById(string id)
        {
            return _gameRepository.FindById(id).Result;
        }

        public List<Game> GetSavedGames(string userId)
        {
            return _gameRepository.GetByHostId(userId);
        }

        public string NewFreeForAll(string lobbyId, string userId, string[] tilePackIds)
        {
            //Get lobby and throw exception if not provided with correct host id
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != userId) throw new Exception("only the host of the lobby can start the game");
            if (tilePackIds.Length <= 0) throw new Exception("You need to choose tilepacks for this gamemode");

            //Check ownership over chosen packages
            if (tilePackIds.Any(tpId =>
                !_ownedTilePackRepository.ConfirmOwnership(new OwnedTilePackEntity(null, userId, tpId))))
                throw new Exception("You dont have ownership over one or more of the tilepacks");
            
            var gameId = _gameRepository.Create(new GameEntity(null, null, userId, null, State.Ongoing)).Result;
            
            var players = _pendingPlayerRepository.GetByLobbyId(lobbyId).Result;

            var packTiles = new List<PackTile>();

            foreach (var packId in tilePackIds)
                packTiles.AddRange(_packTileRepository.GetByPackId(packId).Result);

            var boardTilesPack = new List<BoardTileEntity>();
            foreach (var player in players)
            {
                var board = _boardRepository.Create(new BoardEntity(null, gameId)).Result;
                var boardMemberId = _boardMemberRepository.Insert(new BoardMemberEntity(null, player.User.Id, board));
                var boardTilesPlayer = new List<BoardTileEntity>();
                var packTilesTemp = new List<PackTile>(packTiles);
                while (boardTilesPlayer.Count < 25 && packTilesTemp.Count > 0)
                {
                    
                    if (boardTilesPlayer.Count == 12)
                    {
                        boardTilesPlayer.Add(new BoardTileEntity(null, null, board, null, boardTilesPlayer.Count, null));
                    }
                    else
                    {
                        var randomTile = packTilesTemp[_random.Next(0, packTilesTemp.Count)];
                        boardTilesPlayer.Add(new BoardTileEntity(null, player.User.Id, board, randomTile.Tile.Id,
                            boardTilesPlayer.Count,
                            null));
                        packTilesTemp.Remove(randomTile);
                    }
                }

                boardTilesPack.AddRange(boardTilesPlayer);
            }

            //Insert all boardtiles in database
            var unused = boardTilesPack.Select(boardTile =>
                    _boardTileRepository.Create(boardTile).Result)
                .ToList();

            return gameId;
        }

        public void Update(GameEntity game)
        {
            _gameRepository.Update(game);
        }


        public string NewOG(string lobbyId, string userId, string[]? tilePackIds)
        {
            //Get lobby and throw exception if not provided with correct host id
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != userId) throw new Exception("only the host of the lobby can start the game");

            var gameId = _gameRepository.Create(new GameEntity(null, null, userId, null, State.Ongoing)).Result;
            var players = _pendingPlayerRepository.GetByLobbyId(lobbyId).Result;


            if (tilePackIds is null || tilePackIds.Length < 1)
            {
                //Set board and tiles up for each player in lobby
                foreach (var player in players)
                {
                    //Create board for player
                    var board = _boardRepository.Create(new BoardEntity(null, gameId)).Result;
                    var boardMemberId = _boardMemberRepository.Insert(new BoardMemberEntity(null, player.User.Id, board));

                    //get tiles about other players
                    List<BoardTileEntity> boardTilesUser = _userTileRepository.GetTilesForBoard(lobbyId, player.User.Id)
                        .Result
                        .Select((t, i) => new BoardTileEntity(null, t.User.Id, board, t.Id,  i, null))
                        .ToList();

                    //if not enough boardtiles for users; fill with default tiles
                    if (boardTilesUser.Count < 25)
                    {
                        List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();

                        var defaultTiles = GetDefaultTiles();
                        //create temp list for refilling in case, that not enough default tiles are available
                        var defaultTilesTemp = new List<PackTile>();

                        while (boardTilesUser.Count < 25)
                        {
                            if (boardTilesUser.Count == 12)
                            {
                                boardTilesUser.Add(new BoardTileEntity(null, null, board, null, boardTilesUser.Count, null));
                            }
                            else
                            {

                                //Make sure there is enough default tiles
                                if (defaultTilesTemp.Count < 1) defaultTilesTemp = new List<PackTile>(defaultTiles);

                                //find random player
                                var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;

                                //find random default tile
                                var randomTile = defaultTilesTemp[_random.Next(0, defaultTilesTemp.Count)];

                                //Create new board tile
                                boardTilesUser.Add(
                                    new BoardTileEntity(null, rp.Id, board, randomTile.Tile.Id, boardTilesUser.Count,
                                        null));

                                //remove random default tile after used, to avoid duplicate boardtiles
                                defaultTilesTemp.Remove(randomTile);
                            }
                        }
                    }

                    //Insert all boardtiles in database
                    var unused = boardTilesUser.Select(boardTile =>
                            _boardTileRepository.Create(boardTile).Result)
                        .ToList();
                }
            }
            else
            {
                
                //Check ownership over chosen packages
                if (tilePackIds.Any(tpId =>
                    !_ownedTilePackRepository.ConfirmOwnership(new OwnedTilePackEntity(null, userId, tpId))))
                    throw new Exception("You dont have ownership over one or more of the tilepacks");

                var packTiles = new List<PackTile>();

                foreach (var packId in tilePackIds)
                    packTiles.AddRange(_packTileRepository.GetByPackId(packId).Result);

                var boardTilesPack = new List<BoardTileEntity>();
                foreach (var player in players)
                {
                    List<PendingPlayer> usablePlayers = players.Where(pp => pp.Id != player.Id).ToList();
                    var board = _boardRepository.Create(new BoardEntity(null, gameId)).Result;
                    var boardTilesPlayer = new List<BoardTileEntity>();
                    var packTilesTemp = new List<PackTile>(packTiles);
                    var boardMemberId = _boardMemberRepository.Insert(new BoardMemberEntity(null, player.User.Id, board));
                    while (boardTilesPlayer.Count < 25 && packTilesTemp.Count > 0)
                    {
                        if (boardTilesPlayer.Count == 12)
                        {
                            boardTilesPlayer.Add(new BoardTileEntity(null, null, board, null, boardTilesPlayer.Count, null));
                        }
                        else
                        {
                            //find random player
                            var rp = usablePlayers[_random.Next(0, usablePlayers.Count)].User;

                            var randomTile = packTilesTemp[_random.Next(0, packTilesTemp.Count)];
                            boardTilesPlayer.Add(new BoardTileEntity(id: null, aboutUserId: rp.Id, boardId: board,
                                tileId: randomTile.Tile.Id, position: boardTilesPlayer.Count,
                                activatedBy: null));
                            packTilesTemp.Remove(randomTile);
                        }
                    }

                    boardTilesPack.AddRange(boardTilesPlayer);
                }

                //Insert all boardtiles in database
                var unused = boardTilesPack.Select(boardTile =>
                        _boardTileRepository.Create(boardTile).Result)
                    .ToList();
            }

            return gameId;
        }

        public string NewShared(string lobbyId, string userId, string[] tilePacks)
        {
            //Get lobby and throw exception if not provided with correct host id
            var lobby = _lobbyRepository.FindById(lobbyId).Result;
            if (lobby.Host != userId) throw new Exception("only the host of the lobby can start the game");
            
            if (tilePacks.Length < 1)
            {
                throw new Exception("At least one tilePack is needed to start this type of game");
            }
            
            var players = _pendingPlayerRepository.GetByLobbyId(lobby.Id).Result;
            var gameId = _gameRepository.Create(new GameEntity(null, null,lobby.Host, null, State.Ongoing)).Result;
            var boardId = _boardRepository.Create(new BoardEntity(null, gameId)).Result;

            var boardTiles = new List<BoardTileEntity>();
            
            var packTiles = new List<PackTile>();

            foreach (var pack in tilePacks)
                packTiles.AddRange(_packTileRepository.GetByPackId(pack).Result);

            while (boardTiles.Count <25)
            {
                if (boardTiles.Count == 12)
                {
                    boardTiles.Add(new BoardTileEntity(null, null, boardId, null, boardTiles.Count, null));
                }
                else
                {
                    var randomTile = packTiles[_random.Next(0, packTiles.Count)];
                    boardTiles.Add(new BoardTileEntity(null, null, boardId, randomTile.Tile.Id, boardTiles.Count, null));
                    
                    packTiles.Remove(randomTile);
                }
            }
            
            //Insert all boardtiles in database
            var unused = boardTiles.Select(boardTile =>
                    _boardTileRepository.Create(boardTile).Result)
                .ToList();
            
            foreach (var player in players)
            {
                var boardMemberId = _boardMemberRepository.Insert(new BoardMemberEntity(null, player.User.Id, boardId));
            }

            return gameId;
        }


        /// <exception cref="Exception">if the user is not on the list</exception>
        public List<User> GetPlayers(string gameId, string userId)
        {
            var players = _userRepository.GetPlayers(gameId);

            if (players.Any(u => u.Id == userId)) return players;

            throw new Exception("You cannot get player list for a game, that you are not a part of");
        }

        public void Delete(string gameId, string hostId)
        {
            var game = _gameRepository.FindById(gameId).Result;
            if (game.Host.Id == hostId)
                _gameRepository.Delete(gameId);
            else
                throw new Exception("You have to be the host of a game to delete it");
        }

        public Game ConfirmWin(string gameId, string hostId)
        {
            var game = _gameRepository.FindById(gameId).Result;
            if (game.Host.Id != hostId) throw new Exception("Only the host can Confirm a win");

            game.State = State.Ended;
            var topRanked = _boardRepository.FindTopRanking(gameId, 3).Result;
            /*
            foreach (var board in topRanked)
            {
                var user = _userRepository.ReadById(board.UserId);
                var unused = _topPlayerRepository.Create(new TopPlayerEntity(null, gameId, user.Id, board.TurnedTiles))
                    .Result;
            }
            */
                Console.WriteLine("ended");
            _gameRepository.Update(new GameEntity(game)).Wait();
            return _gameRepository.FindById(gameId).Result;
            
        }

        public void PauseGame(GameEntity game)
        {
            game.State = State.Paused;

            _gameRepository.Update(game).Wait();
        }

        public Game DenyWin(string gameId, string userId)
        {
            var game = _gameRepository.FindById(gameId).Result;

            if (game.Host.Id != userId) throw new Exception("Only the host can deny wins");

            game.State = State.Ongoing;
            game.WinnerId = null;

            _gameRepository.Update(new GameEntity(game)).Wait();
            return _gameRepository.FindById(gameId).Result;
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