using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [Authorize]
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
    }
}