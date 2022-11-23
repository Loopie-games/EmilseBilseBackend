using System.Data;

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

        public User(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("User_Id"));
            Username = reader.GetString(reader.GetOrdinal("User_Username"));
            Nickname = reader.GetString(reader.GetOrdinal("User_Nickname"));
            ProfilePicUrl = reader.GetValue(reader.GetOrdinal("User_ProfilePicUrl")).ToString();
        }
        
        public User(IDataReader reader, int start)
        {
            Id = reader.GetString(start);
            Username = reader.GetString(start+1);
            Nickname = reader.GetString(start+2);
            ProfilePicUrl = reader.GetValue(start+3).ToString();
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string? ProfilePicUrl { get; set; }
    }
}