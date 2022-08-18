using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;

namespace moonbaboon.bingo.Domain.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;

        public FriendshipService(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
        {
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }
        
        public List<Friendship> GetAll()
        {
            return _friendshipRepository.FindAll().Result;  
        }

        public List<Friend> GetFriendsByUserId(string userId)
        {
            List<Friend> friends = new();

            foreach (var friendship in _friendshipRepository.FindAcceptedFriendshipsByUserId(userId).Result)
            {
                if (friendship.FriendId1.Id == userId)
                {
                    friends.Add(new Friend(friendship.Id, friendship.FriendId2, true));
                }
                else if(friendship.FriendId2.Id == userId)
                {
                    friends.Add(new Friend(friendship.Id, friendship.FriendId1, true));
                }
            }
            return friends;
        }

        public Friendship? SendFriendRequest(string fromUserId, string toUserId)
        {
            //check is users er identical
            if (fromUserId == toUserId)
            {
                //Todo feedback "you cant send yourself a friendRequest"
                return null;
            }
            //check if user exists
            if (_userRepository.ReadById(toUserId).Result is null)
            {
                //Todo feedback "the user you are requesting does not exist"
                return null;
            }
            //checks if already friends
            if (_friendshipRepository.ValidateFriendship(fromUserId, toUserId).Result)
            {
                //Todo feedback "is already Friends"
                return null;
            }
            return _friendshipRepository.Create(fromUserId, toUserId, false).Result;
        }

        public List<Friendship> GetFriendRequestsByUserId(string userId)
        {
            return _friendshipRepository.FindFriendRequests_ByUserId(userId).Result;
        }

        public Friend AcceptFriendRequest(string friendshipId, string acceptingUserId)
        {
            var friendship = _friendshipRepository.FindById(friendshipId).Result;
            if (friendship is {Accepted: true})
            {
                throw new Exception("This friendship is already accepted");
            }
            if (friendship?.FriendId2.Id != acceptingUserId)
            {
                throw new Exception("This request is not for you to accept");
            }
            friendship.Accepted = true;
            var friendshipUpdated = _friendshipRepository.AcceptFriendship(friendshipId).Result;
            if (friendshipUpdated is not null)
            {
                return new Friend(friendshipUpdated.Id, friendship.FriendId1, friendshipUpdated.Accepted);
            }
            throw new Exception("Something went wrong when accepting friendship");
        }
    };
}