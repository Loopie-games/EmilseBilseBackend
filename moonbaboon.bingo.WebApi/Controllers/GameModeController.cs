using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameModeController : ControllerBase
    {
        private readonly IGameModeService _gameModeService;

        public GameModeController(IGameModeService gameModeService)
        {
            _gameModeService = gameModeService;
        }

        [HttpGet]
        public ActionResult<List<GameMode>> GetAll()
        {
            try
            {
                return Ok(_gameModeService.GetAll());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
    }
}