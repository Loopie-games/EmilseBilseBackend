using System;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.Core.Models;
using Moq;
using Xunit;

namespace moonbaboon.bingo.Domain.Test.IService;

public class ILobbyServiceTest
{
    private readonly Mock<ILobbyService> _service = new Mock<ILobbyService>();

    [Fact]
    public void ILobbyService_IsAvailable()
    {
        Assert.NotNull(_service.Object);
    }

    [Fact]
    public void ILobbyService_GetById_NotNull()
    {
        string uuid = Guid.NewGuid().ToString();
        LobbyForUser lobby = new LobbyForUser(uuid, "<TEST>", "test1");
        _service.Setup(service => service.GetById(uuid)).Returns(lobby);
        Assert.NotNull(_service.Object.GetById(uuid));
    }
    
    [Fact]
    public void ILobbyService_GetById()
    {
        string uuid = Guid.NewGuid().ToString();
        LobbyForUser lobby = new LobbyForUser(uuid, "<TEST>", "test1");
        _service.Setup(service => service.GetById(uuid)).Returns(lobby);
        Assert.Equal(lobby, _service.Object.GetById(uuid));
    }
    
    [Fact]
    public void ILobbyService_GetById_NullInput()
    {
        string uuid = Guid.NewGuid().ToString();
        LobbyForUser lobby = new LobbyForUser(uuid, "<TEST>", "test1");
        _service.Setup(service => service.GetById(uuid)).Returns(lobby);
        Assert.NotEqual(lobby, _service.Object.GetById(null));
    }

    [Fact]
    public void ILobbyService_Create_NotNull()
    {
        string hostUUID = Guid.NewGuid().ToString();
        Lobby lobbyToCreate = new Lobby(hostUUID);
        _service.Setup(service => service.Create(lobbyToCreate)).Returns(lobbyToCreate);
        Assert.NotNull(_service.Object.Create(lobbyToCreate));
    }

    [Fact]
    public void ILobbyService_Create()
    {
        string hostUUID = Guid.NewGuid().ToString();
        Lobby lobbyToCreate = new Lobby(hostUUID);
        _service.Setup(service => service.Create(lobbyToCreate)).Returns(lobbyToCreate);
        Assert.Equal(lobbyToCreate, _service.Object.Create(lobbyToCreate));
    }

    [Fact]
    public void ILobbyService_Create_NullInput()
    {
        string hostUUID = Guid.NewGuid().ToString();
        Lobby lobbyToCreate = new Lobby(hostUUID);
        _service.Setup(service => service.Create(lobbyToCreate)).Returns(lobbyToCreate);
        Assert.NotEqual(lobbyToCreate, _service.Object.Create(null));

    }
}