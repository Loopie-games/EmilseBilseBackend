using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IGameService
    {
        public Game? GetById(string id);
        
        public Game? Create(string hostId);
        public Game NewGame(Lobby lobby);
        /// <summary>
        /// Gets player list from game id
        /// Checks if the user requesting the list is part of the game
        /// if the user is part of the game, the player list is returned
        /// </summary>
        /// <param name="gameId">Id specific for a Game</param>
        /// <param name="userId">Id specific for a User</param>
        /// <returns>List of Players participating in the game</returns>
        public List<UserSimple> GetPlayers(string gameId, string userId);
    }
}