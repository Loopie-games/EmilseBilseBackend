using System.Collections.Generic;
using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserTileService
    {
        public List<UserTile> GetAll();
        public UserTile? GetById(string id);
        public List<UserTile> GetAboutUserById(string id);
        public UserTile NewTile(UserTileEntity toCreate);
        public List<UserTile> GetMadeByUserId(string userId);
    }
}