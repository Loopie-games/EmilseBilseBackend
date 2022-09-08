namespace moonbaboon.bingo.Core.Models
{
    public class Board
    {
        public Board(string? id, string userId, string gameId)
        {
            Id = id;
            UserId = userId;
            GameId = gameId;
        }

        public string? Id { get; set; }
        public string UserId { get; set; }
        public string GameId { get; set; }
    }
}