using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    //[Authorize]
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
        
        [HttpGet(nameof(Search) + "/{searchStr}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserSimple>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<UserSimple>> Search(string searchStr)
        {
            if (searchStr.Length > 2)
            {
                return Ok(_userService.Search(searchStr));
            }
            else
            {
                return BadRequest("use at last 3 characters in your search");
            }
            
            
        }

        [Authorize]
        [HttpGet(nameof(GetLogged))]
        public ActionResult<UserSimple> GetLogged()
        {
            try
            {
                return _userService.GetById(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
            
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

        [AllowAnonymous]
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

        [HttpGet(nameof(GetSalt) + "/{username}")]
        public ActionResult<string?> GetSalt(string username)
        {
            var u = _userService.GetSalt(username);
            return u;
        }

    }
}