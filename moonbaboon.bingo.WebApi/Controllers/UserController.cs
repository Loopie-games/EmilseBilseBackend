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
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet(nameof(Search) + "/{searchStr}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<User>> Search(string searchStr)
        {
            if (searchStr.Length > 2) return Ok(_userService.Search(searchStr));
            return BadRequest("use at last 3 characters in your search");
        }

        [Authorize]
        [HttpGet(nameof(GetLogged))]
        public ActionResult<User> GetLogged()
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
        public ActionResult<User> GetById(string id)
        {
            try
            {
                var user = _userService.GetById(id);
                return Ok(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound();
            }
        }

        [HttpGet(nameof(VerifyUsername))]
        public IActionResult VerifyUsername(string username)
        {
            return _userService.UsernameExists(username)
                ? new JsonResult($"Username '{username}' is already in use.")
                : new JsonResult(true);
        }

        [AllowAnonymous]
        [HttpPost(nameof(CreateUser))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> CreateUser(UserDtos.NewUserDto user)
        {
            try
            {
                if (_userService.UsernameExists(user.Username))
                    return BadRequest($"Username '{user.Username}' is already in use.");

                var u = _userService.CreateUser(user.ToUser());
                var a = _authService.Create(new AuthEntity(null, u, user.Password, user.Salt));
                return CreatedAtAction(nameof(GetById), new {id = u}, u);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpGet(nameof(GetSalt) + "/{username}")]
        public ActionResult<string?> GetSalt(string username)
        {
            try
            {
                var userId = _userService.GetUserIdByUsername(username);
                if (userId != null) return Ok(_userService.GetSalt(userId));

                return BadRequest("User with given username does not exist");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public ActionResult<User> UpdateUser(User user)
        {
            try
            {
                _userService.UpdateUser(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, user);
                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e);
            }
        }
    }
}