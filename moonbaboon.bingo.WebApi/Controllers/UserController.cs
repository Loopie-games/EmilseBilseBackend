using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            return _userService.GetAll();
        }

        [HttpGet("{id}")]
        public ActionResult<User?> GetById(string id)
        {
            var user = _userService.GetById(id);
            return user;
        }

        [HttpPost(nameof(CreateUser))]
        public ActionResult<UserDto> CreateUser(CreateUserDto user)
        {
            return new UserDto(_userService.CreateUser(new User(user.UserName, user.Password, user.NickName)));
        }
        
        public class CreateUserDto
        {
            public CreateUserDto(string userName, string nickName, string password)
            {
                UserName = userName;
                NickName = nickName;
                Password = password;
            }

            public string UserName { get; set; }
            public string NickName { get; set; }
            public string Password { get; set; }
        }
        
        public class UserDto
        {
            public UserDto(User u)
            {
                Id = u.Id;
                UserName = u.Username;
                NickName = u.Nickname;
            }

            public string? Id { get; set; }
            public string UserName { get; set; }
            public string NickName { get; set; }
            
            
        }


        [HttpPost(nameof(Login))]
        public ActionResult<LoginResponse> Login(LoginDto dto)
        {
            var user = _userService.Login(dto.Username, dto.Password);
            return user is {Id: { }} ? new LoginResponse(true, user.Id) : new LoginResponse(false, "null");
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