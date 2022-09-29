using MySqlConnector;

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

        public Board(MySqlDataReader reader)
        {
            Id = reader.GetString("Board_Id");
            GameId = reader.GetString("Board_GameId");
            UserId = reader.GetString("Board_UserId");
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public string UserId { get; set; }
        public int TurnedTiles { get; set; }
    }
}