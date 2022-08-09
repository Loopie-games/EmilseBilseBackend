using moonbaboon.bingo.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITileRepository
    {
        public Task<Tile?> Create(Tile tileToCreate);
        public Task<List<Tile>> FindAll();
        public Task<Tile?> FindById(string id);
        public Task<bool> Delete(string id);
        public Task<List<Tile>> GetAboutUserById(string id);
        public Task<List<TileForUser>> GetAboutUserById_TileForUser(string id);
        public Task<TileForUser?> CreateTile_TileForUser(Tile tileToCreate);
    }
}
