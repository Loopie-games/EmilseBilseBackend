using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserService
    {
        public List<User> Search(string searchStr);
        public List<User> SearchID(string searchStr);
        public User? Login(string dtoUsername, string dtoPassword);
        public User GetById(string id);
        public User CreateUser(User user);
        public bool VerifyUsername(string username);
        public string GetSalt(string username);
        public User UpdateUser(string id, User user);
        bool RemoveBanner(string uuid, string adminUUID);
        bool RemoveIcon(string uuid, string adminUUID);
        bool RemoveName(string uuid, string adminUUID);
    }
}