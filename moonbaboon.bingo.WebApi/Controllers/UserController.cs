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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<User>> Search(string searchStr)
        {
            if (searchStr.Length > 2) return Ok(_userService.Search(searchStr));
            return BadRequest("use at last 3 characters in your search");
        }

        [HttpGet(nameof(SearchID) + "/{searchStr}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<User>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<User>> SearchID(string searchStr)
        {
            if (searchStr.Length > 2) return Ok(_userService.SearchID(searchStr));
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
            return !_userService.VerifyUsername(username)
                ? new JsonResult($"Username '{username}' is already in use.")
                : new JsonResult(true);
        }

        [AllowAnonymous]
        [HttpPost(nameof(CreateUser))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> CreateUser(User user)
        {
            try
            {
                if (!_userService.VerifyUsername(user.Username))
                    return BadRequest($"Username '{user.Username}' is already in use.");

                var u = _userService.CreateUser(user);
                return CreatedAtAction(nameof(GetById), new { id = u.Id }, u);
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

        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(string id, User user) {

            if (id != user.Id)
                return BadRequest("User ID mismatch...");

            User res = _userService.UpdateUser(id, user);


            return Ok(res);
        }

        [HttpPut(nameof(RemoveBanner) + "/{uuid}")]
        public ActionResult<bool> RemoveBanner(string uuid, string adminUUID){
            bool res = false;//_userService.RemoveBanner(uuid, adminUUID);
            return res ? Ok() : BadRequest();
        }

        [HttpPut(nameof(RemoveIcon) + "/{uuid}")]
        public ActionResult<bool> RemoveIcon(string uuid, string adminUUID){
            bool res = false;//_userService.RemoveIcon(uuid, adminUUID);
            return res ? Ok() : BadRequest();
        }

        [HttpPut(nameof(RemoveName) + "/{uuid}")]
        public ActionResult<bool> RemoveName(string uuid, string adminUUID){
            Console.WriteLine($"Incoming: {uuid} FROM {adminUUID}");
            bool res = _userService.RemoveName(uuid, adminUUID);
            return res ? Ok() : BadRequest();
        }
    }
}