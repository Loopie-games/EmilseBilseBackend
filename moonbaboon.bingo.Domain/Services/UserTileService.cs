using System;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System.Collections.Generic;

namespace moonbaboon.bingo.Domain.Services
{
    public class UserTileService : IUserTileService
    {
        private readonly IUserTileRepository _userTileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public UserTileService(IUserTileRepository userTileRepository, IUserRepository userRepository, IFriendshipRepository friendshipRepository)
        {
            _userTileRepository = userTileRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public UserTile? Create(string userId, string action, string addedById)
        {
            return _userTileRepository.Create(userId, action, addedById).Result;
        }

        public List<UserTile> GetAll()
        {
            return _userTileRepository.FindAll().Result;
        }

        public UserTile? GetById(string id)
        {
            return _userTileRepository.FindById(id).Result;
        }

        public bool DeleteTile(string id)
        {
            return _userTileRepository.Delete(id).Result;
        }

        public List<UserTile> GetAboutUserById(string id)
        {
            return _userTileRepository.GetAboutUserById(id).Result;
        }

        public UserTile NewTile(string tileAboutUserId, string tileAction, string tileAddedByUserId)
        {
            var user = _userRepository.ReadById(tileAboutUserId).Result;
            if (user?.Id is null)
            {
                throw new Exception($"The {nameof(User)} you are trying to add a {nameof(UserTile)} to, does not exist");
            }
            var isFriends = _friendshipRepository.ValidateFriendship(user.Id, tileAddedByUserId).Result;
            if (!isFriends)
            {
                throw new Exception($"You need to be friends to add tiles");
            }
            var tile = _userTileRepository.Create(user.Id, tileAction, tileAddedByUserId).Result;
            if (tile is not null)
            {
                return tile;
            }
            throw new Exception($"Something went wrong when creating the tile");
        }

        public List<UserTile> GetMadeByUserId(string userId)
        {
            return _userTileRepository.FindMadeByUserId(userId).Result;
        }
    }
}
