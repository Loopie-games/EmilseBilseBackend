namespace moonbaboon.bingo.Core.Models
{
    public class TopPlayer
    {
        public TopPlayer(string? id, string gameId, User user, int turnedTiles)
        {
            Id = id;
            GameId = gameId;
            User = user;
            TurnedTiles = turnedTiles;
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public User User { get; set; }
        public int TurnedTiles { get; set; }
    }
}