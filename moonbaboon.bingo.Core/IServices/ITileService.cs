using moonbaboon.bingo.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Core.IServices
{
    public interface ITileService
    {
        public List<Tile> GetAll();
        public Tile? GetById(string id);
        public Tile? CreateTile(Tile tileToCreate);
        public bool DeleteTile(string id);
    }
}
