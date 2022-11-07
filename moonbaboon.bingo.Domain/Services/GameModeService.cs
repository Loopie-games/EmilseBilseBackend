using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class GameModeService: IGameModeService
    {
        private readonly IGameModeRepository _gameModeRepository;

        public GameModeService(IGameModeRepository gameModeRepository)
        {
            _gameModeRepository = gameModeRepository;
        }

        public List<GameMode> GetAll()
        {
            return _gameModeRepository.FindAll().Result;
        }
    }
}