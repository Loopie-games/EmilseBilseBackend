using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserRepository
    {
        public Task<List<User>> Search(string searchString);
        public Task<List<User>> SearchID(string searchString);
        public Task<User> Login(string dtoUsername, string dtoPassword);

        /// <summary>
        ///     Finds the user with the given id in the database
        /// </summary>
        /// <param name="id">Specific user Id</param>
        /// <returns>A Task with the User as Result</returns>
        public Task<User> ReadById(string id);

        public Task<User> Create(User user);
        public Task<bool> VerifyUsername(string username);
        public Task<string> GetSalt(string username);
        public Task<User> UpdateUser(string id, User user);
        public Task<bool> RemoveBanner(string uuid);
        public Task<bool> RemoveIcon(string uuid);
        public Task<bool> RemoveName(string uuid);
        
        /// <summary>
        /// Gets list of players is in game with given Id
        /// </summary>
        /// <param name="gameId">Id specific for game</param>
        /// <returns>Player list</returns>
        public Task<List<User>> GetPlayers(string gameId);
    }
}