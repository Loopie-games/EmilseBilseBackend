using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OwnedTilePackController : ControllerBase
    {
        private readonly IOwnedTilePackService _tilePackService;

        public OwnedTilePackController(IOwnedTilePackService tilePackService)
        {
            _tilePackService = tilePackService;
        }

        [Authorize]
        [HttpGet(nameof(GetOwned))]
        public ActionResult<List<OwnedTilePack>> GetOwned()
        {
            try
            {
                return _tilePackService.GetOwnedTilePacks(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest(e.Message);
            }
        }
    }
}