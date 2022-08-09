using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [Authorize]
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDtos.UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDtos.UserDto> CreateUser(UserDtos.CreateUserDto user)
        {
            if (!_userService.VerifyUsername(user.UserName))
            {
                return BadRequest($"Username '{user.UserName}' is already in use.");
            }

            var u = new User(user.UserName, user.Password, user.Salt, user.NickName);
            if (!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                u.ProfilePicUrl = user.ProfilePicUrl;
            }
            return  new UserDtos.UserDto(_userService.CreateUser(u));

        }


        [HttpPost(nameof(Login))]
        public ActionResult<UserDtos.LoginResponse> Login(UserDtos.LoginDto dto)
        {
            var user = _userService.Login(dto.Username, dto.Password);
            return user is {Id: { }} ? new UserDtos.LoginResponse(true, user.Id) : new UserDtos.LoginResponse(false, "null");
        }

        [HttpGet(nameof(GetSalt))]
        public ActionResult<string?> GetSalt(string username)
        {
            return _userService.GetSalt(username);
        }

    }
}