using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TileController:ControllerBase
    {
        private readonly ITileRepository _tileRepository;

        public TileController(ITileRepository tileRepository)
        {
            _tileRepository = tileRepository;
        }

        [HttpGet]
        public ActionResult<List<Tile>> GetAll()
        {
            return _tileRepository.GetAll().Result;
        }
    }
}