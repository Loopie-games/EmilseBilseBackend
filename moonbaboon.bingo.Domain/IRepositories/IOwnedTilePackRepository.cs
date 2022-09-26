using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IOwnedTilePackRepository
    {
        public Task<List<OwnedTilePack>> GetOwnedTilePacks(string userId);
    }
}