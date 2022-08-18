using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public FriendshipService(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
            
        }
        
        public List<Friendship> GetAll()
        {
            return _friendshipRepository.FindAll().Result;  
        }

        public List<Friend> GetFriendsByUserId(string userId)
        {
            return _friendshipRepository.FindAcceptedFriendshipsByUserId(userId).Result;
        }

        public Friendship? SendFriendRequest(string fromUserId, string toUserId)
        {
            //checks if already friends
            if (_friendshipRepository.ValidateFriendship(fromUserId, toUserId).Result)
            {
                //Todo feedback "is already Friends"
                return null;
            }
            return _friendshipRepository.Create(fromUserId, toUserId, false).Result;
        }
    };
}