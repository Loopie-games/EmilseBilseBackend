using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IPackTileService
    {
        public List<PackTile> GetByPackId(string packId);
        public PackTile Create(Tile tile, string packId);
        public PackTile GetById(string id);
    }
}