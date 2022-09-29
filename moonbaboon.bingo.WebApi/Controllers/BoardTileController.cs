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
    public class BoardTileController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IBoardTileService _boardTileService;

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
            try
            {
                var boardTiles = _boardTileService.GetByBoardId(id);
                return Ok(boardTiles);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
            
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
        public ActionResult<BoardTile> Create(BoardTileEntity boardTile)
        {
            try
            {
                var created = _boardTileService.Create(boardTile);
                return CreatedAtAction(nameof(GetById), new {created.Id}, created);
            }
            catch (Exception e)
            {
                Console.Write(e);
                return BadRequest(e.Message);
            }
        }
    }
}