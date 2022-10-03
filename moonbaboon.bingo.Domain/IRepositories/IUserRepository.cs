using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserRepository
    {
        public Task<List<UserSimple>> Search(string searchString);
        public Task<UserSimple> SearchID(string searchString);
        public Task<UserSimple> Login(string dtoUsername, string dtoPassword);

        /// <summary>
        ///     Finds the user with the given id in the database
        /// </summary>
        /// <param name="id">Specific user Id</param>
        /// <returns>A Task with the User as Result</returns>
        public Task<UserSimple> ReadById(string id);

        public Task<UserSimple> Create(User user);
        public Task<bool> VerifyUsername(string username);
        public Task<string> GetSalt(string username);
        public Task<UserSimple> UpdateUser(string id, UserSimple user);
    }
}