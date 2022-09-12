using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopPlayerController : ControllerBase
    {
        private readonly ITopPlayerService _topPlayerService;

        public TopPlayerController(ITopPlayerService topPlayerService)
        {
            _topPlayerService = topPlayerService;
        }

        [HttpGet(nameof(FindTop3))]
        public ActionResult<List<TopPlayer>> FindTop3(string gameId)
        {
            return _topPlayerService.FindTop(gameId, 3);
        }
    }
}