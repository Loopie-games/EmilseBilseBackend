using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class PackTileService: IPackTileService
    {
        private readonly IPackTileRepository _packTileRepository;

        public PackTileService(IPackTileRepository packTileRepository)
        {
            _packTileRepository = packTileRepository;
        }

        public List<PackTile> GetByPackId(string packId)
        {
            return _packTileRepository.GetByPackId(packId).Result;
        }
    }
}