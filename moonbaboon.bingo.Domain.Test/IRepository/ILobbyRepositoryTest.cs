using System;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IRepository;

public class ILobbyRepositoryTest
{
    private readonly Mock<ILobbyRepository> _repo = new Mock<ILobbyRepository>();

    [Fact]
    public void ILobbyRepository_IsAvailable()
    {
        Assert.NotNull(_repo.Object);
    }

    [Fact]
    public void ILobbyRepository_Create_IsAvailable()
    {
        Assert.NotNull(_repo.Object.Create(new Lobby(Guid.NewGuid().ToString())));
    }

    [Fact]
    public void ILobbyRepository_Create_ReturnsLobby()
    {
        string hostUUID = Guid.NewGuid().ToString();
        Lobby lobby = new(hostUUID);
        _repo.Setup(s => s.Create(lobby).Result).Returns(lobby);

        var actual = _repo.Object.Create(lobby).Result;
        
        Assert.Equal(lobby, actual);
    }

    [Fact]
    public void ILobbyRepository_Create_ReturnsNull()
    {
        Assert.Null(_repo.Object.Create(new Lobby("NOT-A-VALID-UUID")).Result);
    }

    [Fact]
    public void ILobbyRepository_FindById_IsAvailable()
    {
        Assert.NotNull(_repo.Object.FindById(""));
    }

    [Fact]
    public void ILobbyRepository_FindById_ReturnsLobby()
    {
        string uuid = Guid.NewGuid().ToString();
        LobbyForUser expected = new LobbyForUser(uuid, Guid.NewGuid().ToString(), "TEST1");
        _repo.Setup(s => s.FindById(uuid).Result).Returns(expected);

        var actual = _repo.Object.FindById(uuid).Result;
        
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ILobbyRepository_FindById_ReturnsNothing()
    {
        Assert.Null(_repo.Object.FindById("not a valid uuid").Result);
    }
}