using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TileController : ControllerBase
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

        [HttpGet("{id}")]
        public ActionResult<Tile?> GetById(string id) {
            return _tileService.GetById(id);
        }
        [HttpGet(nameof(GetAboutUserById))]
        public ActionResult<List<Tile>> GetAboutUserById(string id)
        {
            return _tileService.GetAboutUserById(id);
        }
        

        [HttpPost(nameof(Create))]
        public ActionResult<Tile?> Create(NewTileDto newTile)
        {
            Tile? t = _tileService.NewTile(newTile.AboutUserName, newTile.Action, newTile.AddedByUserId);
            return t;
        }
        
        

        [HttpPost(nameof(Delete))]
        public ActionResult<bool> Delete(string id)
        {
            return _tileService.DeleteTile(id);
        }
    }
}
