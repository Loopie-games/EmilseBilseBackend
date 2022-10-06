using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, IAdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public List<UserSimple> Search(string searchStr)
        {
            return _userRepository.Search(searchStr).Result;
        }

        public List<UserSimple> SearchID(string searchStr)
        {
            return _userRepository.SearchID(searchStr).Result;
        }

        public UserSimple Login(string dtoUsername, string dtoPassword)
        {
            return _userRepository.Login(dtoUsername, dtoPassword).Result;
        }

        public UserSimple GetById(string id)
        {
            var u = _userRepository.ReadById(id).Result;
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

        public UserSimple UpdateUser(string id, UserSimple user)
        {
            return _userRepository.UpdateUser(id, user).Result;
        }

        public bool RemoveBanner(string uuid, string adminUUID)
        {
            if(_adminRepository.IsAdmin(adminUUID).Result != null){
                return _userRepository.RemoveBanner(uuid).Result;       
            }
            return false;
        }

        public bool RemoveIcon(string uuid, string adminUUID)
        {
            if(_adminRepository.IsAdmin(adminUUID).Result != null){
                return _userRepository.RemoveIcon(uuid).Result;       
            }
            return false;
        }

        public bool RemoveName(string uuid, string adminUUID)
        {
            if(_adminRepository.IsAdmin(adminUUID).Result != null){
                return _userRepository.RemoveName(uuid).Result;       
            }
            return false;
        }
    }
}