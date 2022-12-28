using System;
using System.Data;

namespace moonbaboon.bingo.Core.Models
{
    public class User
    {
        public User(string? id, string username, string nickname, string? profilePicUrl, string? bannerPicUrl, string email, DateTime birthDate)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
            ProfilePicUrl = profilePicUrl;
            BannerPicUrl = bannerPicUrl;
            Email = email;
            BirthDate = birthDate;
        }

        public User(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("User_Id"));
            Username = reader.GetString(reader.GetOrdinal("User_Username"));
            Nickname = reader.GetString(reader.GetOrdinal("User_Nickname"));
            ProfilePicUrl = reader.GetValue(reader.GetOrdinal("User_ProfilePicUrl")).ToString();
            BannerPicUrl = reader.GetValue(reader.GetOrdinal("User_BannerPicUrl")).ToString();
            Email = reader.GetString(reader.GetOrdinal("User_Email"));
            BirthDate = reader.GetDateTime(reader.GetOrdinal("User_Birthdate"));
        }
        
        public User(IDataReader reader, int start)
        {
            Id = reader.GetString(start);
            Username = reader.GetString(start+1);
            Nickname = reader.GetString(start+2);
            ProfilePicUrl = reader.GetValue(start+3).ToString();
            BannerPicUrl =  reader.GetValue(start+4).ToString();
            Email = reader.GetString(start + 5);
            BirthDate = reader.GetDateTime(start + 6);
        }

        public string? Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string? ProfilePicUrl { get; set; }
        
        public string? BannerPicUrl { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
    }
}