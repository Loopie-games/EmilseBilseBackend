using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserService
    {
        public List<User> Search(string searchStr);
        public User? Login(string dtoUsername, string dtoPassword);
        public User GetById(string id);
        public string CreateUser(User user);
        public bool VerifyUsername(string username);
        public string GetSalt(string username);
        public void UpdateUser(string id, User user);
    }
}