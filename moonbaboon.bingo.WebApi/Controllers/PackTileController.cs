using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackTileController:ControllerBase
    {
        private readonly IPackTileService _packTileService;

        public PackTileController(IPackTileService packTileService)
        {
            _packTileService = packTileService;
        }
        
        [HttpGet(nameof(GetByPackId) + "/{packId}")]
        public ActionResult<List<PackTile>> GetByPackId(string packId)
        {
            return _packTileService.GetByPackId(packId);
        }
    }
}