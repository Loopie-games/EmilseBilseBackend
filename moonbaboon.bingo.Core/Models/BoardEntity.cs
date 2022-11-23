using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardEntity
    {
        public BoardEntity(string? id, string gameId, string? userId)
        {
            Id = id;
            GameId = gameId;
            UserId = userId;
        }

        public BoardEntity(MySqlDataReader reader)
        {
            Id = reader.GetString("Board_Id");
            GameId = reader.GetString("Board_GameId");
            UserId = reader.GetValue("Board_UserId").ToString();
            //TODO turnedtiles
        }

        public BoardEntity(IDataReader reader)
        {
            Id = reader.GetString(reader.GetOrdinal("Board_Id"));
            GameId = reader.GetString(reader.GetOrdinal("Board_GameId"));
            UserId = reader.GetValue(reader.GetOrdinal("Board_UserId")).ToString();
            //Todo turnedtiles
        }

        public string? Id { get; set; }
        public string GameId { get; set; }
        public string? UserId { get; set; }
        public int TurnedTiles { get; set; }
    }
}