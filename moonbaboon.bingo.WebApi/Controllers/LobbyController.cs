using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController: ControllerBase
    {
        private readonly ILobbyService _lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        [HttpGet("{lobbyId}")]
        public ActionResult<Lobby?> GetById(string lobbyId)
        {
            return _lobbyService.GetById(lobbyId);
        }

        [HttpPost]
        public ActionResult<Lobby?> Create(Lobby lobby)
        {
            return _lobbyService.Create(lobby);
        }
    }
}