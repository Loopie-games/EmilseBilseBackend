using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace moonbaboon.bingo.WebApi.Controllers
{
    public class CreateUserDto
        {
            
            private const int MaxNameLength = 25;
            private const int MinNameLength = 8;
            private const int MinNickNameLength = 3;
            private const int MaxPasswordLength = 30;
            private const int MinPasswordLength = 5;
            
            public CreateUserDto(string userName, string nickName, string password)
            {
                UserName = userName;
                NickName = nickName;
                Password = password;
            }
            

            [Required]
            [StringLength(MaxNameLength, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = MinNameLength)]
            [DefaultValue($"{nameof(UserName)}")]
            public string UserName { get; set; }
            [Required]
            [StringLength(MaxNameLength, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = MinNickNameLength)]
            [DefaultValue($"{nameof(NickName)}")]
            public string NickName { get; set; }
            [Required]
            [StringLength(MaxPasswordLength, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = MinPasswordLength)]
            [DefaultValue($"{nameof(Password)}")]
            public string Password { get; set; }
        }
    }