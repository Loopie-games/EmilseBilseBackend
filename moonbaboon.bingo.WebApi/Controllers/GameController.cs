using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController: ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }
        
        [HttpGet("{id}")]
        public ActionResult<Game?> GetById(string id)
        {
            return _gameService.GetById(id);
        }

        [HttpPost(nameof(Create))]
        public ActionResult<Game?> Create(string hostId)
        {
            return _gameService.Create(hostId);
        }
    }
}