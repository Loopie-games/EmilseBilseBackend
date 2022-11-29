namespace moonbaboon.bingo.WebApi.DTOs
{
    public class GameDtos
    {
        public class CreateGameDto
        {
            public CreateGameDto(string lobbyId, string name, string[]? tpIds)
            {
                LobbyId = lobbyId;
                Name = name;
                TpIds = tpIds;
            }

            public string LobbyId { get; set; }
            public string Name { get; set; }
            public string[]? TpIds { get; set; }
        }
        
        public class GameNameChangeDto
        {
            public GameNameChangeDto(string gameId, string name)
            {
                GameId = gameId;
                Name = name;
            }

            public string GameId { get; set; }
            public string? Name { get; set; }
        }
    }
}