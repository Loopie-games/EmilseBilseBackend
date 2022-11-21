﻿using System;
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

        public List<User> Search(string searchStr)
        {
            return _userRepository.Search(searchStr).Result;
        }

        public User Login(string dtoUsername, string dtoPassword)
        {
            return _userRepository.Login(dtoUsername, dtoPassword).Result;
        }

        public User GetById(string id)
        {
            return _userRepository.ReadById(id).Result;
        }

        public string CreateUser(User user)
        {
            return _userRepository.Create(user).Result;
        }

        public bool VerifyUsername(string username)
        {
            return _userRepository.UsernameExists(username).Result;
        }

        public string GetSalt(string username)
        {
            return _userRepository.GetSalt(username).Result;
        }

        public void UpdateUser(string id, User user)
        {
            if (id == user.Id)
            {
                _userRepository.UpdateUser(user).Wait();
            }
            else
            {
                throw new Exception("You can only change your own profile");
            }

        }

        public void RemoveName(string uuid, string adminUUID)
        {
            if(_adminRepository.FindByUserId(adminUUID).Result != null){
                 _userRepository.RemoveName(uuid).Wait();       
            }
        }
    }
}