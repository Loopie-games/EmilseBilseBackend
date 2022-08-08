using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserRepository
    {
        public Task<List<User>> FindAll();
        public Task<User?> Login(string dtoUsername, string dtoPassword);
        public  Task<User?> ReadById(string id);
        public Task<User> Create(User user);
        public Task<bool> VerifyUsername(string username);
        public Task<string?> GetSalt(string username);
    }
}