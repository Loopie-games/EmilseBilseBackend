﻿using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IGameService
    {
        /// <summary>
        /// Find a game object from an Id
        /// </summary>
        /// <param name="id"> Id string</param>
        /// <returns>Game with the given id</returns>
        public Game GetById(string id);

        /// <summary>
        ///     Creates a new Game from lobby, if provided with id from matching lobby and host.
        /// </summary>
        /// <param name="lobbyId">Id specific for a lobby</param>
        /// <param name="hostId">UserId from the host of the lobby</param>
        /// <param name="tilePackIds"></param>
        /// <returns>the created game</returns>
        public string NewGame(string lobbyId, string hostId, string[]? tilePackIds);

        /// <summary>
        ///     Gets player list from game id
        ///     Checks if the user requesting the list is part of the game
        ///     if the user is part of the game, the player list is returned
        /// </summary>
        /// <param name="gameId">Id specific for a Game</param>
        /// <param name="userId">Id specific for a User</param>
        /// <returns>List of Players participating in the game</returns>
        public List<UserSimple> GetPlayers(string gameId, string userId);

        /// <summary>
        /// Deletes a game object, if given matching host and id of the game
        /// </summary>
        /// <param name="gameId">The id of the game</param>
        /// <param name="hostId">the id of the host</param>
        /// <returns>bool representing the completion of the deletion</returns>
        public bool Delete(string gameId, string hostId);

        /// <summary>
        /// Host can confirm that a game is won
        /// </summary>
        /// <param name="gameId">Id of the game</param>
        /// <param name="hostId">id of the host</param>
        /// <returns>the game</returns>
        public Game ConfirmWin(string gameId, string hostId);
        
        /// <summary>
        /// Pauses the game
        /// </summary>
        /// <param name="game">the game</param>
        /// <param name="userId"> id of the player that wants to pause</param>
        /// <returns>The paused game</returns>
        public Game PauseGame(Game game, string userId);
        
        /// <summary>
        /// The host can deny that a game is won
        /// </summary>
        /// <param name="gameId">id of the game</param>
        /// <param name="userId">id of the player that wants to deny</param>
        /// <returns>the game</returns>
        public Game DenyWin(string gameId, string userId);
    }
}