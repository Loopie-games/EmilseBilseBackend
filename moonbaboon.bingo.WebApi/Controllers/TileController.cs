using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
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
        
        [HttpGet(nameof(GetAboutUserById_TileForUser))]
        public ActionResult<List<TileForUser>> GetAboutUserById_TileForUser(string id)
        {
            return _tileService.GetAboutUserById_TileForUser(id);
        }

        [HttpPost(nameof(Create))]
        public ActionResult<CreateResponse?> Create(Tile tile)
        {
            Tile? t = _tileService.CreateTile(tile);
            return (t != null) ? new CreateResponse(t.UserId, t.Action) : null;
        }

        [HttpPost(nameof(Delete))]
        public ActionResult<bool> Delete(string id)
        {
            return _tileService.DeleteTile(id);
        }

        public class CreateResponse {
            public string UserId;
            public string Action;

            public CreateResponse(string userId, string action)
            {
                Action = action;
                UserId = userId;
            }
        }
    }
}
