using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;

        public UserService(IUserRepository userRepository, IAdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public List<UserSimple> Search(string searchStr)
        {
            return _userRepository.Search(searchStr).Result;
        }

        public UserSimple Login(string dtoUsername, string dtoPassword)
        {
            return _userRepository.Login(dtoUsername, dtoPassword).Result;
        }

        public UserSimple GetById(string id)
        {
            var u =  _userRepository.ReadById(id).Result;
            var a = _adminRepository.IsAdmin(u.Id).Result;
            return a ?? u;
        }

        public UserSimple CreateUser(User user)
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