using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserService
    {
        
        public List<User> GetAll();
        public List<UserSimple> Search(string searchStr);
        public User? Login(string dtoUsername, string dtoPassword);
        public UserSimple GetById(string id);
        public User CreateUser(User user);
        public bool VerifyUsername(string username);
        public string? GetSalt(string username);
    }
}