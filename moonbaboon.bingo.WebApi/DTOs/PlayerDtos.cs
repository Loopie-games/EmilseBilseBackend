using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class PendingPlayerDto
    {
        public PendingPlayerDto(PendingPlayer pp)
        {
            Id = pp.Id;
            Player = pp.User;
            LobbyId = pp.Lobby.Id;
        }

        public string Id { get; set; }
        
        public string LobbyId { get; set; }
        public UserSimple Player { get; set; }
    }
}