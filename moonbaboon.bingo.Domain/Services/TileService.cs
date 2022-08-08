using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System;
using System.Collections.Generic;
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

        public bool DeleteTile(string id)
        {
            return _tileRepository.Delete(id).Result;
        }

        public List<Tile> GetAboutUserById(string id)
        {
            return _tileRepository.GetAboutUserById(id).Result;
        }

        public List<TileForUser> GetAboutUserById_TileForUser(string id)
        {
            return _tileRepository.GetAboutUserById_TileForUser(id).Result;
        }
    }
}
