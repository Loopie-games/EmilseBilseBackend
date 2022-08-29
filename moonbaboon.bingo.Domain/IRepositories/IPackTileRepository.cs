using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IPackTileRepository
    {
        public Task<List<PackTile>> GetByPackId(string packId);
    }
}