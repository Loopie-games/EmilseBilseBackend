using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TilePackController:ControllerBase
    {
        private readonly ITilePackService _tilePackService;

        public TilePackController(ITilePackService tilePackService)
        {
            _tilePackService = tilePackService;
        }
        
        [HttpGet]
        public ActionResult<List<TilePack>> GetAll()
        {
            return _tilePackService.GetAll();
        }
    }
}