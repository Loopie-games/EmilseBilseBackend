using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Lobby
    {
        public Lobby(string? id, string host, string? pin)
        {
            Id = id;
            Host = host;
            Pin = pin;
        }

        public Lobby(MySqlDataReader reader)
        {
            Id = reader.GetString("Lobby_Id");
            Host = reader.GetString("Lobby_Host");
            Pin = reader.GetString("Lobby_Pin");
        }

        public string? Id { get; set; }

        public string Host { get; set; }

        public string? Pin { get; set; }
    }
}