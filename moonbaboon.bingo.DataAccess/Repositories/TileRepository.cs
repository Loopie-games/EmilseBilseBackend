using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class TileRepository : ITileRepository
    {
        public Task<Tile?> Create(Tile tileToCreate)
        {
            throw new NotImplementedException();
        }

        public Task<Tile?> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Tile>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<Tile?> FindById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
