using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IPackTileService
    {
        public List<PackTile> GetByPackId(string packId);
        public PackTile GetById(string id);
        public PackTileEntity Create(PackTileEntity pt);
        public bool Clear(string id);
    }
}