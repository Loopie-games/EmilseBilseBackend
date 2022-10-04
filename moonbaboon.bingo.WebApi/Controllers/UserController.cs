using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet(nameof(Search) + "/{searchStr}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserSimple>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<UserSimple>> Search(string searchStr)
        {
            if (searchStr.Length > 2) return Ok(_userService.Search(searchStr));
            return BadRequest("use at last 3 characters in your search");
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSimple))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserSimple> GetById(string id)
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
            return !_userService.VerifyUsername(username)
                ? new JsonResult($"Username '{username}' is already in use.")
                : new JsonResult(true);
        }

        [AllowAnonymous]
        [HttpPost(nameof(CreateUser))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserSimple))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserSimple> CreateUser(User user)
        {
            try
            {
                if (!_userService.VerifyUsername(user.Username))
                    return BadRequest($"Username '{user.Username}' is already in use.");

                var u = _userService.CreateUser(user);
                return CreatedAtAction(nameof(GetById), new {id = u.Id}, u);
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
                return Ok(_userService.GetSalt(username));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}