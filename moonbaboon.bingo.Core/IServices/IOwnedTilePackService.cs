using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IOwnedTilePackService
    {
        public List<OwnedTilePack> GetOwnedTilePacks(string userId);
        bool IsOwned(OwnedTilePackEntity entity);
    }
}