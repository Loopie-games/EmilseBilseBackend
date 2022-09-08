using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class OwnedTilePackRepository : IOwnedTilePackRepository
    {
        public Task<List<OwnedTilePack>> GetOwnedTilePacks(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}