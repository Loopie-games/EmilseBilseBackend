using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace moonbaboon.bingo.WebApi.DTOs
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

            public string Salt { get; set; }
        }
    }