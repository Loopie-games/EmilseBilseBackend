using System;
using System.Collections.Generic;
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
        

        public List<User> Search(string searchStr)
        {
            return _userRepository.Search(searchStr);
        }

        public User Login(string dtoUsername, string dtoPassword)
        {
            return _userRepository.Login(dtoUsername, dtoPassword);
        }

        public User GetById(string id)
        {
            return _userRepository.ReadById(id);
        }

        public string CreateUser(User user)
        {
            return _userRepository.Insert(user);
        }

        public bool UsernameExists(string username)
        {
            var b = _userRepository.GetUserIdByUsername(username);
            return b is not null;
        }

        public string GetSalt(string userId)
        {
            return _userRepository.GetSalt(userId);
        }

        public void UpdateUser(string id, User user)
        {
            if (id == user.Id)
                _userRepository.UpdateUser(user);
            else
                throw new Exception("You can only change your own profile");
        }

        public string? GetUserIdByUsername(string username)
        {
            return _userRepository.GetUserIdByUsername(username);
        }

        public List<User> GetBoardMembers(string boardId)
        {
            return _userRepository.GetBoardMembers(boardId);
        }
    }
}