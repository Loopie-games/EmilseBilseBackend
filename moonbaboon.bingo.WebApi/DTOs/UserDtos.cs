using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class UserDtos
    {

        public class CreateUserDto
        {
            public CreateUserDto(string userName, string nickName, string password, string salt)
            {
                UserName = userName;
                NickName = nickName;
                Password = password;
                Salt = salt;
            }
            
            public string UserName { get; set; }
            public string NickName { get; set; }
            public string Password { get; set; }
            public string Salt { get; set; }
            public string? ProfilePicUrl { get; set; }


        }

        public class UserDto
        {
            public UserDto(User u)
            {
                Id = u.Id;
                Username = u.Username;
                Nickname = u.Nickname;
                ProfilePicUrl = u.ProfilePicUrl;
            }

            public string? Id { get; set; }
            public string Username { get; set; }
            public string Nickname { get; set; }

            public string? ProfilePicUrl { get; set; }
        }

        public class LoginResponse
        {
            public LoginResponse(bool isValid, string userId)
            {
                IsValid = isValid;
                UserId = userId;
            }
            public bool IsValid { get; set; }
            public string UserId { get; set; }
        }
        
        public class LoginDto
        {
            public LoginDto(string username, string password)
            {
                Username = username;
                Password = password;
            }

            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}