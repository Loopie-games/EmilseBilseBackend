using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

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
            return Ok(_userService.GetAll());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User?> GetById(string id)
        {
            var user = _userService.GetById(id);
            if (user != null)
            {
               return Ok(user); 
            }

            return NotFound();
        }
        [HttpGet(nameof(VerifyUsername))]
        public IActionResult VerifyUsername(string username)
        {
            return !_userService.VerifyUsername(username) ? new JsonResult($"Username '{username}' is already in use.") : new JsonResult(true);
        }

        [HttpPost(nameof(CreateUser))]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User?> CreateUser(CreateUserDto user)
        {
            if (!_userService.VerifyUsername(user.UserName))
            {
                return BadRequest($"Username '{user.UserName}' is already in use.");
            }
            
            var userCreated = new UserDto(_userService.CreateUser(new User(user.UserName, user.Password, user.NickName)));
            return CreatedAtAction(nameof(GetById), new {id = userCreated.Id}, userCreated);
        }

        public class UserDto
        {
            public UserDto(User u)
            {
                Id = u.Id;
                Username = u.Username;
                Nickname = u.Nickname;
            }

            public string? Id { get; set; }
            public string Username { get; set; }
            public string Nickname { get; set; }
            
            
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