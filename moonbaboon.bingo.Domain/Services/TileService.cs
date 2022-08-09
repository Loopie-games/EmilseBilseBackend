using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        public Tile? CreateTile(Tile tileToCreate)
        {
            return _tileRepository.Create(tileToCreate).Result;
        }

        public List<Tile> GetAll()
        {
            return _tileRepository.FindAll().Result;
        }

        public Tile? GetById(string id)
        {
            return _tileRepository.FindById(id).Result;
        }

        public TileForUser? CreateTile_TileForUser(TileNewFromUser tileToCreate)
        {
            //getting UserId
            var aboutUser = _userRepository.GetByUsername(tileToCreate.AboutUserName).Result;
            if (aboutUser is null)
            {
                return null;
            }
            
            //validating friendship between users
            if (!_friendshipRepository.ValidateFriendship(aboutUser.Id, tileToCreate.AddedByUserId).Result)
            {
                return null;
            }
            return _tileRepository.CreateTile_TileForUser(tileToCreate);
        }

        public bool DeleteTile(string id)
        {
            return _tileRepository.Delete(id).Result;
        }

        public List<Tile> GetAboutUserById(string id)
        {
            return _tileRepository.GetAboutUserById(id).Result;
        }

        public List<TileForUser> GetAboutUserById_TileForUser(string id)
        {
            return _tileRepository.GetAboutUserById_TileForUser(id).Result;
        }
    }
}
