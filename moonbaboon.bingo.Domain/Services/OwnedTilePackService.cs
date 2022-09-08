using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class OwnedTilePackService : IOwnedTilePackService
    {
        private readonly IOwnedTilePackRepository _ownedTilePackRepository;

        public OwnedTilePackService(IOwnedTilePackRepository ownedTilePackRepository)
        {
            _ownedTilePackRepository = ownedTilePackRepository;
        }

        public List<OwnedTilePack> GetOwnedTilePacks(string userId)
        {
            return _ownedTilePackRepository.GetOwnedTilePacks(userId).Result;
        }
    }
}