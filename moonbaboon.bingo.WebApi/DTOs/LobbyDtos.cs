using moonbaboon.bingo.Core.Models;

namespace moonbaboon.bingo.WebApi.DTOs
{
    public class LobbyForPlayerDto
    {
        public LobbyForPlayerDto(string lobbyId, string pin, UserSimple host)
        {
            LobbyId = lobbyId;
            Pin = pin;
            Host = host;
        }

        public string LobbyId { get; set; }
        public UserSimple Host { get; set; }
        public string Pin { get; set; }
    }
}