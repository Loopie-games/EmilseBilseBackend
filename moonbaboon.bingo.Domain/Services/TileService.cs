using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class TileService : ITileService
    {
        private readonly ITileRepository _tileRepository;

        public TileService(ITileRepository tileRepository)
        {
            _tileRepository = tileRepository;
        }

        public List<Tile> GetAll()
        {
            return _tileRepository.GetAll().Result;
        }
    }
}