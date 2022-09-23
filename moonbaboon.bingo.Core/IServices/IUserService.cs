﻿using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserService
    {
        public List<UserSimple> Search(string searchStr);
        public UserSimple? Login(string dtoUsername, string dtoPassword);
        public UserSimple GetById(string id);
        public UserSimple CreateUser(User user);
        public bool VerifyUsername(string username);
        public string GetSalt(string username);
        public UserSimple UpdateUser(string id, User user);
    }
}