using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserTileRepository
    {
        public Task<UserTile?> Create(string userId, string action, string addedById);
        public Task<List<UserTile>> FindAll();
        public Task<UserTile?> FindById(string id);
        public Task<UserTile?> FindFiller(string userId);
        public Task<List<UserTile>> GetAboutUserById(string id);
        public Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId);
        public Task<List<UserTile>> FindMadeByUserId(string userId);
    }
}