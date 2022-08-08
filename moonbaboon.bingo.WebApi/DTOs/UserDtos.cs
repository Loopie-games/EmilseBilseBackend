using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class UserDtos
    {

        public class CreateUserDto
        {

            private const int MaxNameLength = 25;
            private const int MinNameLength = 8;
            private const int MinNickNameLength = 3;
            private const int MaxPasswordLength = 30;
            private const int MinPasswordLength = 5;
            private const string ErrorMessageNameLength = "{0} length must be between {2} and {1} Characters.";

            public CreateUserDto(string userName, string nickName, string password)
            {
                UserName = userName;
                NickName = nickName;
                Password = password;
            }


            [Required]
            [StringLength(MaxNameLength, ErrorMessage = ErrorMessageNameLength, MinimumLength = MinNameLength)]
            [DefaultValue($"{nameof(UserName)}")]
            public string UserName { get; set; }

            [Required]
            [StringLength(MaxNameLength, ErrorMessage = ErrorMessageNameLength, MinimumLength = MinNickNameLength)]
            [DefaultValue($"{nameof(NickName)}")]
            public string NickName { get; set; }

            [Required]
            [StringLength(MaxPasswordLength, ErrorMessage = ErrorMessageNameLength, MinimumLength = MinPasswordLength)]
            [DefaultValue($"{nameof(Password)}")]
            public string Password { get; set; }

            [Required] public string Salt { get; set; }

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