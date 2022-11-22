using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class User
    {
        public User(string? id, string username, string nickname, string? profilePicUrl)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
            ProfilePicUrl = profilePicUrl;
        }

        public User(MySqlDataReader reader)
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