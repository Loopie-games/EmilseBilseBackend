using moonbaboon.bingo.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface ITileRepository
    {
        public Task<Tile?> Create(string userId, string action, string addedById);
        public Task<List<Tile>> FindAll();
        public Task<Tile?> FindById(string id);
        public Task<Tile?> FindFiller(string userId);
        public Task<bool> Delete(string id);
        public Task<List<Tile>> GetAboutUserById(string id);
        public Task<List<Tile>> GetTilesForBoard(string lobbyId, string userId);
    }
}
