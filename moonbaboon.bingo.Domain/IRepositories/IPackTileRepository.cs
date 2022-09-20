using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IPackTileRepository
    {
        public Task<List<PackTile>> GetByPackId(string packId);
        public Task<PackTile> Create(PackTile toCreate);
        public Task<PackTile> GetById(string id);
        public Task<List<Tile>> GetTilesUsedInPacks();
    }
}