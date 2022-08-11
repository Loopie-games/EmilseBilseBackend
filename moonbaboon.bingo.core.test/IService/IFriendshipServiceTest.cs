using System;
using System.Collections.Generic;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using Moq;
using Xunit;


namespace moonbaboon.bingo.Core.Test.IService
{

    public class IFriendshipServiceTest
    {
        private readonly Mock<IFriendshipService> _service = new Mock<IFriendshipService>();

        [Fact]
        public void IFriendshipService_IsAvailable()
        {
            Assert.NotNull(_service.Object);
        }

        [Fact]
        public void IFriendshipService_GetAll()
        {
            List<Friendship> friendships = new List<Friendship>();

            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            _service.Setup(s => s.GetAll()).Returns(friendships);
            
            Assert.Equal(friendships, _service.Object.GetAll());
        }
        
        [Fact]
        public void IFriendshipService_GetAll_NotNull()
        {
            List<Friendship> friendships = new List<Friendship>();

            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            friendships.Add(new Friendship(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            _service.Setup(s => s.GetAll()).Returns(friendships);
            
            Assert.NotNull(_service.Object.GetAll());
        }

        [Fact]
        public void IFriendService_GetFriendsByUserId()
        {
            List<Friend> listOfFriends = new List<Friend>();
            listOfFriends.Add(new Friend("",""));
        }

        
        [Fact]
        public void IFriendService_GetFriendsByUserId_NotNull(){}
        
        [Fact]
        public void IFriendService_GetFriendsByUserId_NullInput(){}

    }
}