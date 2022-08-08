using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetAll()
        {
            return _userRepository.FindAll().Result;
        }

        public User? Login(string dtoUsername, string dtoPassword)
        {
            return _userRepository.Login(dtoUsername, dtoPassword).Result;
        }

        public User? GetById(string id)
        {
            return _userRepository.ReadById(id).Result;
        }

        public User CreateUser(User user)
        {
            return _userRepository.Create(user).Result;
        }

        public bool VerifyUsername(string username)
        {
            return _userRepository.VerifyUsername(username).Result;
        }

        public string GetSalt(string username)
        {
            return _userRepository.GetSalt(username).Result;
        }
    }
}