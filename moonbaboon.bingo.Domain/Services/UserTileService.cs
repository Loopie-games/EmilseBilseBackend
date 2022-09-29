using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class UserTileService : IUserTileService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTileRepository _userTileRepository;

        public UserTileService(IUserTileRepository userTileRepository, IUserRepository userRepository,
            IFriendshipRepository friendshipRepository)
        {
            _userTileRepository = userTileRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public List<UserTile> GetAll()
        {
            return _userTileRepository.FindAll().Result;
        }

        public UserTile? GetById(string id)
        {
            return _userTileRepository.FindById(id).Result;
        }


        public List<UserTile> GetAboutUserById(string id)
        {
            return _userTileRepository.GetAboutUserById(id).Result;
        }

        public UserTile NewTile(UserTileEntity toCreate)
        {
            var user = _userRepository.ReadById(toCreate.AboutUserId).Result;
            if (user.Id is null)
                throw new Exception(
                    $"The {nameof(User)} you are trying to add a {nameof(UserTile)} to, does not exist");
            var isFriends = _friendshipRepository.ValidateFriendship(user.Id, toCreate.AddedByUserId).Result;
            if (!isFriends) throw new Exception("You need to be friends to add tiles");
            var tile = _userTileRepository.Create(toCreate).Result;
            return _userTileRepository.FindById(tile.Id).Result;
        }

        public List<UserTile> GetMadeByUserId(string userId)
        {
            return _userTileRepository.FindMadeByUserId(userId).Result;
        }
    }
}