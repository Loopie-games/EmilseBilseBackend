using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Tile
    {
        public Tile(string? id, string action)
        {
            Id = id;
            Action = action;
        }

        public Tile(MySqlDataReader reader)
        {
            Id = reader.GetString("Tile_Id");
            Action = reader.GetString("Tile_Action");
        }

        public string? Id { get; set; }
        public string Action { get; set; }
    }
}