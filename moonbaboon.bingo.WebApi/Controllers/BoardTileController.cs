using System.Collections.Generic;
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
        private readonly ITileService _tileService;

        public BoardTileController(IBoardTileService boardTileService, ITileService tileService)
        {
            _boardTileService = boardTileService;
            _tileService = tileService;
        }

        [HttpGet("{id}")]
        public ActionResult<BoardTile?> GetById(string id)
        {
            return _boardTileService.GetById(id);
        }
        
        [HttpGet(nameof(GetByBoardId) + "/{id}")]
        public ActionResult<List<BoardTileDto>> GetByBoardId(string id)
        {
            var boardTiles = _boardTileService.GetByBoardId(id);
            List<BoardTileDto> list = new();

            foreach (var boardTile in boardTiles)
            {
                var tile = _tileService.GetById(boardTile.TileId);
                list.Add(new BoardTileDto(boardTile.Id, boardTile.Board.Id, new TileDto(tile), boardTile.Position, boardTile.IsActivated));
            }

            return list;
        }

        [HttpPost(nameof(Create))]
        public ActionResult<BoardTile?> Create(BoardTile boardTile)
        {
            return _boardTileService.Create(boardTile);
        }
    }
}