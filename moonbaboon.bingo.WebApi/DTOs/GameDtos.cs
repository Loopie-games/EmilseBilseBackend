namespace moonbaboon.bingo.WebApi.DTOs
{
    public class StartGameDtos
    {
        public StartGameDtos(string lobbyId, string userId)
        {
            LobbyId = lobbyId;
            UserId = userId;
        }

        public string LobbyId { get; set; }
        public string UserId { get; set; }
    }
}