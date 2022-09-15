﻿using System;
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

        public List<Friend> SearchUsers(string searchStr, string loggedUserId)
        {
            return _friendshipRepository.SearchUsers(searchStr, loggedUserId).Result;
        }

        public List<Friend> GetFriendsByUserId(string userId)
        {
            List<Friend> friends = new();

            var repoFriends = _friendshipRepository.FindAcceptedFriendshipsByUserId(userId).Result;
            foreach (var friendship in repoFriends)
            {
                var f = FriendshipToFriend(friendship, userId);
                if (f is not null) friends.Add(f);
            }

            return friends;
        }

        public Friend? SendFriendRequest(string fromUserId, string toUserId)
        {
            //check is users er identical
            if (fromUserId == toUserId) throw new Exception("you cant send yourself a friendRequest");
            //check if user exists
            if (_userRepository.ReadById(toUserId).Result is null)
                throw new Exception("the user you are requesting does not exist");
            //checks if already friends
            if (_friendshipRepository.ValidateFriendship(fromUserId, toUserId).Result)
                throw new Exception("is already Friends");

            var friendship = _friendshipRepository.Create(fromUserId, toUserId, false).Result;
            if (friendship is null) throw new Exception("friendship was not created");

            return FriendshipToFriend(friendship, fromUserId);
        }

        public List<Friend> GetFriendRequestsByUserId(string userId)
        {
            List<Friend> friendRequests = new();
            foreach (var request in _friendshipRepository.FindFriendRequests_ByUserId(userId).Result)
            {
                var f = FriendshipToFriend(request, userId);
                if (f is not null) friendRequests.Add(f);
            }

            return friendRequests;
        }

        public Friend AcceptFriendRequest(string friendshipId, string acceptingUserId)
        {
            var friendship = _friendshipRepository.FindById(friendshipId).Result;
            if (friendship is {Accepted: true}) throw new Exception("This friendship is already accepted");
            if (friendship?.FriendId2.Id != acceptingUserId)
                throw new Exception("This request is not for you to accept");
            friendship.Accepted = true;
            var friendshipUpdated = _friendshipRepository.AcceptFriendship(friendshipId).Result;
            if (friendshipUpdated is not null)
                return new Friend(friendshipUpdated.Id, friendship.FriendId1, friendshipUpdated.Accepted);
            throw new Exception("Something went wrong when accepting friendship");
        }

        public Friend? FriendshipToFriend(Friendship friendship, string userId)
        {
            if (friendship.FriendId1.Id == userId)
                return new Friend(friendship.Id, friendship.FriendId2, true);
            if (friendship.FriendId2.Id == userId) return new Friend(friendship.Id, friendship.FriendId1, true);

            return null;
        }
    }
}