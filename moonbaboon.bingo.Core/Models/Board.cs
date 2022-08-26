namespace moonbaboon.bingo.Core.Models
{
    public class Board
    {
        public Board(string gameId, string userId)
        {
            GameId = gameId;
            UserId = userId;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string GameId { get; set; }
    }
}