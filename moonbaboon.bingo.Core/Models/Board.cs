namespace moonbaboon.bingo.Core.Models
{
    public class Board
    {
        public Board(string? id, string gameId, string userId)
        {
            Id = id;
            GameId = gameId;
            UserId = userId;
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public string UserId { get; set; }
    }
}