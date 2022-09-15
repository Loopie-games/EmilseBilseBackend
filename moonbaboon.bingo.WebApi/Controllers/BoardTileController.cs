using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BoardTileController: ControllerBase
    {
        private readonly IBoardTileService _boardTileService;
        private readonly IBoardService _boardService;

        public BoardTileController(IBoardTileService boardTileService, IBoardService boardService)
        {
            _boardTileService = boardTileService;
            _boardService = boardService;
        }

        [HttpGet("{id}")]
        public ActionResult<BoardTile?> GetById(string id)
        {
            return _boardTileService.GetById(id);
        }
        
        
        [HttpGet(nameof(GetByBoardId) + "/{id}")]
        public ActionResult<List<BoardTile>> GetByBoardId(string id)
        {
            var boardTiles = _boardTileService.GetByBoardId(id);
            return boardTiles;
        }
        
        [Authorize]
        [HttpGet(nameof(GetByGameId) + "/{gameId}")]
        public ActionResult<List<BoardTile>> GetByGameId(string gameId)
        {
            var board = _boardService.GetByUserAndGameId(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                gameId);
            var boardTiles = _boardTileService.GetByBoardId(board.Id);

            return boardTiles;
        }

        [HttpPost(nameof(Create))]
        public ActionResult<BoardTile?> Create(BoardTile boardTile)
        {
            return _boardTileService.Create(boardTile);
        }
    }
}