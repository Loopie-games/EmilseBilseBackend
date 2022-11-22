using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class TopPlayer
    {
        public TopPlayer(string? id, Game gameId, User user, int turnedTiles)
        {
            Id = id;
            Game = gameId;
            User = user;
            TurnedTiles = turnedTiles;
        }

        public TopPlayer(MySqlDataReader reader)
        {
            Id = reader.GetString("TopPlayer_Id");
            Game = new Game(reader);
            User = new User(reader);
            TurnedTiles = reader.GetInt32("TopPlayer_TurnedTiles");
        }

        public string? Id { get; set; }
        public Game Game { get; set; }
        public User User { get; set; }
        public int TurnedTiles { get; set; }
    }

    public class TopPlayerEntity
    {
        public TopPlayerEntity(string? id, string gameId, string userId, int turnedTiles)
        {
            Id = id;
            GameId = gameId;
            UserId = userId;
            TurnedTiles = turnedTiles;
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public string UserId { get; set; }
        public int TurnedTiles { get; set; }
    }
}