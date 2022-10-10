namespace moonbaboon.bingo.WebApi.DTOs
{
    public class GameDtos
    {
        public class CreateGameDto
        {
            public CreateGameDto(string lobbyId, string[]? tpIds)
            {
                LobbyId = lobbyId;
                TpIds = tpIds;
            }

            public string LobbyId { get; set; }
            public string[]? TpIds { get; set; }
        }
    }
}