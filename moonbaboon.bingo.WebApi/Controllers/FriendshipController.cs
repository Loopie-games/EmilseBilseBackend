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
    public class FriendshipController: ControllerBase
    {

        private readonly IFriendshipService _friendshipService;

        public FriendshipController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
        }
        
        [HttpGet]
        public ActionResult<List<Friendship>> GetAll()
        {
            return Ok(_friendshipService.GetAll());
        }
        
        [HttpGet(nameof(GetFriendsByUserId))]
        public ActionResult<List<Friend>> GetFriendsByUserId(string userId)
        {
            return Ok(_friendshipService.GetFriendsByUserId(userId) );
        }
        
        [Authorize]
        [HttpGet(nameof(SearchUsers) + "/{searchStr}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserSimple>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Friend>> SearchUsers(string searchStr)
        {
            if (searchStr.Length > 2)
            {
                return Ok(_friendshipService.SearchUsers(searchStr, HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            }
            else
            {
                return BadRequest("use at last 3 characters in your search");
            }
        }

        [Authorize]
        [HttpPost(nameof(SendFriendRequest))]
        public ActionResult<Friend> SendFriendRequest(string toUserId)
        {
            try
            {
                return Ok(_friendshipService.SendFriendRequest(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, toUserId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet(nameof(GetFriendRequests))]
        public ActionResult<List<Friend>> GetFriendRequests()
        {
            return _friendshipService.GetFriendRequestsByUserId(
                HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [Authorize]
        [HttpPut(nameof(AcceptFriendRequest) + "/{friendshipId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Friend))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Friend> AcceptFriendRequest(string friendshipId)
        {
            try
            {
                return _friendshipService.AcceptFriendRequest(friendshipId,
                                                                HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}