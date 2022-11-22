using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserRepository
    {
        public List<User> Search(string searchString);
        public User Login(string username, string password);

        /// <summary>
        ///     Finds the user with the given id in the database
        /// </summary>
        /// <param name="id">Specific user Id</param>
        /// <returns>A Task with the User as Result</returns>
        public User ReadById(string id);

        public string Insert(User user);
        public string? GetUserIdByUsername(string username);
        public string GetSalt(string userId);
        public void UpdateUser(User entity);

        /// <summary>
        ///     Gets list of players is in game with given Id
        /// </summary>
        /// <param name="gameId">Id specific for game</param>
        /// <returns>Player list</returns>
        public List<User> GetPlayers(string gameId);
    }
}