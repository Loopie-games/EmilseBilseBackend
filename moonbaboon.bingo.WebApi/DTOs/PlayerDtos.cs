using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class PendingPlayerDto
    {
        public PendingPlayerDto(string id, UserDtos.UserSimpleDto user, string lobbyId)
        {
            Id = id;
            Player = user;
            LobbyId = lobbyId;
        }

        public PendingPlayerDto(PendingPlayer pp)
        {
            Id = pp.Id;
            Player = new UserDtos.UserSimpleDto(pp.User);
            LobbyId = pp.Lobby.Id;
        }

        public string Id { get; set; }
        
        public string LobbyId { get; set; }
        public UserDtos.UserSimpleDto Player { get; set; }
    }
}