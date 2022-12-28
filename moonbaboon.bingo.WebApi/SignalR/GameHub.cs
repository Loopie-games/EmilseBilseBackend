using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.SignalR
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly IBoardService _boardService;
        private readonly IBoardTileService _boardTileService;
        private readonly IGameService _gameService;
        private readonly IUserService _userService;

        public GameHub(IGameService gameService,
            IBoardService boardService, IBoardTileService boardTileService, IUserService userService)
        {
            _gameService = gameService;
            _boardService = boardService;
            _boardTileService = boardTileService;
            _userService = userService;
        }

        /// <summary>
        ///     Send en Error message to the client
        /// </summary>
        /// <param name="message">Error Message</param>
        private async Task SendError(string message)
        {
            await Clients.Caller.SendAsync("PopUpError", message);
        }

        /// <summary>
        ///     Gets UserId From Authorized user
        /// </summary>
        /// <param name="context"></param>
        /// <returns>userId</returns>
        /// <exception cref="Exception">if user is null</exception>
        /// <exception cref="InvalidOperationException">if user id cant be found</exception>
        private static string GetUserId(HubCallerContext context)
        {
            if (context.User == null) throw new Exception("Could not get user from Context");
            return context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   throw new InvalidOperationException("Could not get userId from Context");
        }

        #region Game

        /// <summary>
        ///     Adds connectionId to group for the game
        /// </summary>
        /// <param name="gameId">Id for a specific game</param>
        public async Task ConnectToGame(string gameId)
        {
            try
            {
                var board = _boardService.GetByUserAndGameId(GetUserId(Context), gameId);
                if (board?.Id is null)
                {
                    await SendError("You are not a part of this game");
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, board.Id);
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                    await Clients.Caller.SendAsync("gameConnected", board);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        /// <summary>
        ///     Turns/(de)Activates the tile
        /// </summary>
        /// <param name="boardTileId">id of the tile</param>
        public async Task TurnTile(string boardTileId)
        {
            try
            {
                var tile = _boardTileService.GetById(_boardTileService.TurnTile(boardTileId, GetUserId(Context)).Id);
                var isWon = _boardService.IsBoardFilled(tile.BoardEntity.Id);
                await Clients.Group(tile.BoardEntity.Id).SendAsync("tileTurned", tile);
                if (isWon) await Clients.Caller.SendAsync("boardFilled", tile.BoardEntity.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        /// <summary>
        ///     Claims Win for User
        /// </summary>
        /// <param name="boardId">Id of the board the user wants to claim is won</param>
        public async Task ClaimWin(string boardId)
        {
            try
            {
                var boardMembers = _userService.GetBoardMembers(boardId);
                if (boardMembers.All(u => u.Id != GetUserId(Context)))
                {
                    await SendError("You are not a member of this board");
                }
                else
                {
                    var board = _boardService.GetById(boardId);
                    var game = _gameService.GetById(board.Game.Id);
                    game.WinnerId = board.Id;
                    _gameService.PauseGame(new GameEntity(game));
                    await Clients.Group(game.Id).SendAsync("updateGame", game);
                    await Clients.User(game.Host.Id!).SendAsync("winnerClaimed", boardMembers);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        public async Task ConfirmWin(string gameId)
        {
            try
            {
                Game game = _gameService.ConfirmWin(gameId, GetUserId(Context));
                await Clients.Group(gameId).SendAsync("updateGame", game);
            }
            catch (Exception e)
            {
                await SendError(e.Message);
                throw;
            }
        }

        public async Task DenyWin(string gameId)
        {
            try
            {
                var game = _gameService.DenyWin(gameId, GetUserId(Context));
                await Clients.Group(gameId).SendAsync("updateGame", game);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendError(e.Message);
            }
        }

        #endregion
    }
}