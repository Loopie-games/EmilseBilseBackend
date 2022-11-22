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

        public List<TilePack> GetAll(string? userId)
        {
              return userId == null
                    ? _tilePackRepository.FindAll().Result
                    : _tilePackRepository.FindAll_LoggedUser(userId).Result;
            
            
        }

        public List<TilePack> GetOwned(string userId)
        {
            return _tilePackRepository.GetOwnedTilePacks(userId).Result;
        }

        public TilePack GetById(string id)
        {
            return _tilePackRepository.FindById(id).Result;
        }

        public TilePack GetDefault()
        {
            return _tilePackRepository.FindDefault().Result;
        }

        public string Create(TilePack toCreate)
        {
            return _tilePackRepository.Create(toCreate).Result;
        }

        public void Update(TilePack toUpdate)
        {
            _tilePackRepository.Update(toUpdate).Wait();
        }

        public void Delete(string packId)
        {
            _tilePackRepository.Delete(packId).Wait();
        }
    }
}