using System.Data;
using MySqlConnector;

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
        public UserSimple(string? id, string username, string nickname, string? profilePicUrl)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
            ProfilePicUrl = profilePicUrl;
        }

        public UserSimple(MySqlDataReader reader)
        {
            Id = reader.GetString("User_Id");
            Username = reader.GetString("User_Username");
            Nickname = reader.GetString("User_Nickname");
            ProfilePicUrl = reader.GetValue("User_ProfilePicUrl").ToString();
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string? ProfilePicUrl { get; set; }
    }
}