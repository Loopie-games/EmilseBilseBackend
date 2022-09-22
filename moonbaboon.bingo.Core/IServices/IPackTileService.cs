using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IPackTileService
    {
        public List<PackTile> GetByPackId(string packId);
        public PackTile Create(string Action, string packId);
        public PackTile GetById(string id);
        public List<Tile> GetTilesUsedInPacks();
        public PackTile AddToPack(PackTileEntity pt);
    }
}