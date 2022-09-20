using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TileController:ControllerBase
    {
        private readonly ITileService _tileService;
        
        public TileController(ITileService tileService)
        {
            _tileService = tileService;
        }

        [HttpGet]
        public ActionResult<List<Tile>> GetAll() 
        {
            return _tileService.GetAll();
        }
    }
}