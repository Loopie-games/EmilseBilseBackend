using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class TilePackService : ITilePackService
    {
        private readonly ITilePackRepository _tilePackRepository;

        public TilePackService(ITilePackRepository tilePackRepository)
        {
            _tilePackRepository = tilePackRepository;
        }

        public List<TilePack> GetAll()
        {
            return _tilePackRepository.FindAll().Result;
        }

        public TilePack GetById(string id)
        {
            return _tilePackRepository.FindById(id).Result;
        }

        public TilePack GetDefault()
        {
            return _tilePackRepository.FindDefault().Result;
        }

        public TilePack Create(string name)
        {
            return _tilePackRepository.Create(name).Result;
        }
    }
}