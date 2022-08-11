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
    public void ILobbyService_Create(){}
    
    [Fact]
    public void ILobbyService_Create_NullInput(){}
}