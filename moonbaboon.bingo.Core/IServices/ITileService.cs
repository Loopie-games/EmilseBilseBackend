using moonbaboon.bingo.Core.Models;
using System.Collections.Generic;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITileService
    {
        public List<Tile> GetAll();
        public Tile? GetById(string id);
        public Tile? Create(string userId, string action, string addedById);
        public bool DeleteTile(string id);
        public List<Tile> GetAboutUserById(string id);
        public Tile NewTile(string tileAboutUserId, string tileAction, string tileAddedByUserId);
    }
}
