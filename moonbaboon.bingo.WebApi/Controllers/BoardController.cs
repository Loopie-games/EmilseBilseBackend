using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet("{id}")]
        public ActionResult<BoardEntity?> GetById(string id)
        {
            return _boardService.GetById(id);
        }

        [Authorize]
        [HttpGet(nameof(GetByGameId) + "/{gameId}")]
        public ActionResult<BoardEntity?> GetByGameId(string gameId)
        {
            return _boardService.GetByUserAndGameId(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                gameId);
        }
    }
}