using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IRepository;

public class IFriendshipRepositoryTest
{
    private readonly Mock<IFriendshipRepository> _repo = new();

    [Fact]
    public void IFriendshipRepository_IsAvailable()
    {
        Assert.NotNull(_repo.Object);
    }

    [Fact]
    public void IFriendshipRepository_FindAll_NotNull()
    {
        List<Friendship> expected = new();
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        _repo.Setup(s => s.FindAll().Result)
            .Returns(expected);
        var actual = _repo.Object.FindAll().Result;
        Assert.NotNull(actual);
    }
    
    [Fact]
    public void IFriendshipRepository_FindAll()
    {
        List<Friendship> expected = new();
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        _repo.Setup(s => s.FindAll().Result)
            .Returns(expected);
        var actual = _repo.Object.FindAll().Result;
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void IFriendshipRepository_FindAll_NotEmpty()
    {
        List<Friendship> expected = new();
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        expected.Add(new Friendship(Guid.NewGuid().ToString(),Guid.NewGuid().ToString()));
        _repo.Setup(s => s.FindAll().Result)
            .Returns(expected);
        var actual = _repo.Object.FindAll().Result;
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void IFriendshipRepository_FindAcceptedFriendshipsByUserId_NotNull()
    {
        string uuid = Guid.NewGuid().ToString();
        _repo.Setup(s => s.FindAcceptedFriendshipsByUserId(uuid).Result)
            .Returns(new List<Friend>());
        Assert.NotNull(_repo.Object.FindAcceptedFriendshipsByUserId(uuid));
    }
    
    [Fact]
    public void IFriendshipRepository_FindAcceptedFriendshipsByUserId_IsAvailable()
    {
        Assert.NotNull(_repo.Object.FindAcceptedFriendshipsByUserId(""));
    }

    [Fact]
    public void IFriendshipRepository_FindAcceptedFriendshipsByUserId()
    {
        string uuid = Guid.NewGuid().ToString();
        List<Friend> expected = new List<Friend>();
        expected.Add(new Friend("Frank","Sinatra"));
        _repo.Setup(s => s.FindAcceptedFriendshipsByUserId(uuid).Result)
            .Returns(expected);
        var actual = _repo.Object.FindAcceptedFriendshipsByUserId(uuid).Result;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IFriendshipRepository_FindAcceptedFriendshipsByUserId_NotInList()
    {
        string uuidNotInList = Guid.NewGuid().ToString();
        List<Friend> expected = new List<Friend>();
        _repo.Setup(s => s.FindAcceptedFriendshipsByUserId(uuidNotInList).Result)
            .Returns(new List<Friend>());
        var actual = _repo.Object.FindAcceptedFriendshipsByUserId(uuidNotInList).Result;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IFriendshipRepository_ValidateFriendship_IsAvailable()
    {
        Assert.NotNull(_repo.Object.ValidateFriendship("",""));
    }

    [Fact]
    public void IFriendshipRepository_ValidateFriendship()
    {
        bool expected = true;

        string user1 = Guid.NewGuid().ToString();
        string user2 = Guid.NewGuid().ToString();

        _repo.Setup(s => s.ValidateFriendship(user1, user2).Result)
            .Returns(true);

        bool actual = _repo.Object.ValidateFriendship(user1, user2).Result;
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IFriendshipRepository_ValidateFriendship_NotFound()
    {
        bool expected = false;

        string user1 = Guid.NewGuid().ToString();
        string user2 = Guid.NewGuid().ToString();

        _repo.Setup(s => s.ValidateFriendship(user1, user2).Result)
            .Returns(false);

        bool actual = _repo.Object.ValidateFriendship(user1, user2).Result;
        
        Assert.Equal(expected, actual);
    }
}