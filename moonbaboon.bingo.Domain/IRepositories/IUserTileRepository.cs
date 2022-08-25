using moonbaboon.bingo.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserTileRepository
    {
        public Task<UserTile?> Create(string userId, string action, string addedById);
        public Task<List<UserTile>> FindAll();
        public Task<UserTile?> FindById(string id);
        public Task<UserTile?> FindFiller(string userId);
        public Task<bool> Delete(string id);
        public Task<List<UserTile>> GetAboutUserById(string id);
        public Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId);
        public Task<List<UserTile>> FindMadeByUserId(string userId);
    }
}
