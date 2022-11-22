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

        public FriendshipService(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public List<Friend> SearchUsers(string searchStr, string loggedUserId)
        {
            return _friendshipRepository.SearchUsers(searchStr, loggedUserId).Result;
        }

        public List<Friend> GetFriendsByUserId(string userId)
        {
            return _friendshipRepository.FindFriendsByUserId(userId).Result;
        }

        public string SendFriendRequest(string fromUserId, string toUserId)
        {
            //check is users er identical
            if (fromUserId == toUserId) throw new Exception("you cant send yourself a friendRequest");
            //checks if already friends
            if (_friendshipRepository.ValidateFriendship(fromUserId, toUserId).Result)
                throw new Exception("is already Friends");

            return _friendshipRepository.Create(new FriendshipEntity(null, fromUserId, toUserId, false)).Result;
        }

        public List<Friend> GetFriendRequestsByUserId(string userId)
        {
            return _friendshipRepository.FindFriendRequests_ByUserId(userId).Result;
        }

        public void AcceptFriendRequest(string friendshipId, string acceptingUserId)
        {
            _friendshipRepository.AcceptFriendship(friendshipId, acceptingUserId).Wait();
        }
    }
}