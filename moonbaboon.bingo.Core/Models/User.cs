using System;

namespace moonbaboon.bingo.Core.Models
{
    public class User
    {
        public User(string username, string password, string nickname)
        {
            Username = username;
            Password = password;
            Nickname = nickname;
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
    }
}