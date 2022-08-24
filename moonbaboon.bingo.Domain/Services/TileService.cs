using System;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System.Collections.Generic;

namespace moonbaboon.bingo.Domain.Services
{
    public class TileService : ITileService
    {
        private readonly ITileRepository _tileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;

        public TileService(ITileRepository tileRepository, IUserRepository userRepository, IFriendshipRepository friendshipRepository)
        {
            _tileRepository = tileRepository;
            _userRepository = userRepository;
            _friendshipRepository = friendshipRepository;
        }

        public Tile? Create(string userId, string action, string addedById)
        {
            return _tileRepository.Create(userId, action, addedById).Result;
        }

        public List<Tile> GetAll()
        {
            return _tileRepository.FindAll().Result;
        }

        public Tile? GetById(string id)
        {
            return _tileRepository.FindById(id).Result;
        }

        public bool DeleteTile(string id)
        {
            return _tileRepository.Delete(id).Result;
        }

        public List<Tile> GetAboutUserById(string id)
        {
            return _tileRepository.GetAboutUserById(id).Result;
        }

        public Tile NewTile(string tileAboutUserId, string tileAction, string tileAddedByUserId)
        {
            var user = _userRepository.ReadById(tileAboutUserId).Result;
            if (user?.Id is null)
            {
                throw new Exception($"The {nameof(User)} you are trying to add a {nameof(Tile)} to, does not exist");
            }
            var isFriends = _friendshipRepository.ValidateFriendship(user.Id, tileAddedByUserId).Result;
            if (!isFriends)
            {
                throw new Exception($"You need to be friends to add tiles");
            }
            var tile = _tileRepository.Create(user.Id, tileAction, tileAddedByUserId).Result;
            if (tile is not null)
            {
                return tile;
            }
            throw new Exception($"Something went wrong when creating the tile");
        }

        public List<Tile> GetMadeByUserId(string userId)
        {
            return _tileRepository.FindMadeByUserId(userId).Result;
        }
    }
}
