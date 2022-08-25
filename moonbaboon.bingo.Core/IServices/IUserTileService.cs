using moonbaboon.bingo.Core.Models;
using System.Collections.Generic;

namespace moonbaboon.bingo.Core.IServices
{
    public interface IUserTileService
    {
        public List<UserTile> GetAll();
        public UserTile? GetById(string id);
        public UserTile? Create(string userId, string action, string addedById);
        public bool DeleteTile(string id);
        public List<UserTile> GetAboutUserById(string id);
        public UserTile NewTile(string tileAboutUserId, string tileAction, string tileAddedByUserId);
        public List<UserTile> GetMadeByUserId(string userId);
    }
}
