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

        public PackTile GetPackTile(PackTileEntity pt)
        {
            return _packTileRepository.GetPackTile(pt).Result;
        }

        public PackTile GetById(string id)
        {
            return _packTileRepository.GetById(id).Result;
        }

        public List<Tile> GetTilesUsedInPacks()
        {
            return _packTileRepository.GetTilesUsedInPacks().Result;
        }

        public PackTileEntity Create(PackTileEntity pt)
        {
            return _packTileRepository.Create(pt).Result;
        }

        public bool Clear(string id)
        {
            return _packTileRepository.Clear(id).Result;
        }
    }
}