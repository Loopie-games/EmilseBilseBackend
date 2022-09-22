using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class PackTileService : IPackTileService
    {
        private readonly IPackTileRepository _packTileRepository;
        private readonly ITilePackRepository _tilePackRepository;

        public PackTileService(IPackTileRepository packTileRepository, ITilePackRepository tilePackRepository)
        {
            _packTileRepository = packTileRepository;
            _tilePackRepository = tilePackRepository;
        }

        public List<PackTile> GetByPackId(string packId)
        {
            return _packTileRepository.GetByPackId(packId).Result;
        }

        public PackTile Create(string action, string packId)
        {
            var pack = _tilePackRepository.FindById(packId).Result;
            return _packTileRepository.Create(new PackTile(null, action, pack)).Result;
        }

        public PackTile GetById(string id)
        {
            return _packTileRepository.GetById(id).Result;
        }

        public List<Tile> GetTilesUsedInPacks()
        {
            return _packTileRepository.GetTilesUsedInPacks().Result;
        }

        public PackTile AddToPack(PackTileEntity pt)
        {
            return _packTileRepository.AddToPack(pt).Result;
        }
    }
}