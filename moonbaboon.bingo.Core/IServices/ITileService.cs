using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITileService
    {
        public List<Tile> GetAll();

        public List<Tile> GetTilesUsedInPacks();
    }
}