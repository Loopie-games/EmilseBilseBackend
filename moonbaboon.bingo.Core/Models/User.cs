using System;

namespace moonbaboon.bingo.Core.Models
{
    public class User
    {
        public User(string username, string password, string salt, string nickname)
        {
            Username = username;
            Password = password;
            Salt = salt;
            Nickname = nickname;
            
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }

        public string? ProfilePicUrl { get; set; }
    }

    public class UserSimple
    {
        public UserSimple(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Nickname = user.Nickname;
            ProfilePicUrl = user.ProfilePicUrl;
        }

        public UserSimple(string? id, string username, string? nickname, string? profilePicUrl)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
            ProfilePicUrl = profilePicUrl;
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string? Nickname { get; set; }
        public string? ProfilePicUrl { get; set; }
    }
}