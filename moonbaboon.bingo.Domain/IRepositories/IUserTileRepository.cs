using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Domain.IRepositories
{
    public interface IUserTileRepository
    {
        public Task<UserTileEntity> Create(UserTileEntity toCreate);
        public Task<List<UserTile>> FindAll();
        public Task<UserTile> FindById(string id);
        public Task<List<UserTile>> GetAboutUserById(string aboutUserId);
        public Task<List<UserTile>> GetTilesForBoard(string lobbyId, string userId);
        public Task<List<UserTile>> FindAddedByUserId(string userId);
    }
}