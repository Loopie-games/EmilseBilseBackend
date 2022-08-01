using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Domain.Services
{
    public class TileService : ITileService
    {
        private readonly ITileRepository _tileRepository;

        public TileService(ITileRepository tileRepository)
        {
            _tileRepository = tileRepository;
        }

        public Tile? CreateTile(Tile tileToCreate)
        {
            return _tileRepository.Create(tileToCreate).Result;
        }

        public List<Tile> GetAll()
        {
            return _tileRepository.FindAll().Result;
        }

        public Tile? GetById(string id)
        {
            return _tileRepository.FindById(id).Result;
        }

        public Tile? DeleteTile(string id)
        {
            return _tileRepository.Delete(id).Result;
        }
    }
}
