using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardEntity
    {
        public BoardEntity(string? id, string gameId, string userId)
        {
            Id = id;
            GameId = gameId;
            UserId = userId;
        }

        public BoardEntity(MySqlDataReader reader)
        {
            Id = reader.GetString("Board_Id");
            GameId = reader.GetString("Board_GameId");
            UserId = reader.GetString("Board_UserId");
            //TODO turnedtiles
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public string UserId { get; set; }
        public int TurnedTiles { get; set; }
    }
}