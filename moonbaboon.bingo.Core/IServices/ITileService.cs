using moonbaboon.bingo.Core.Models;
using System.Collections.Generic;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITileService
    {
        public List<Tile> GetAll();
        public Tile? GetById(string id);
        public TileForUser? CreateTile_TileForUser(TileNewFromUser tileToCreate);
        public bool DeleteTile(string id);
        public List<Tile> GetAboutUserById(string id);
        public List<TileForUser> GetAboutUserById_TileForUser(string id);
    }
}
