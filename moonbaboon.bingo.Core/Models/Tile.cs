using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class Tile
    {
        public Tile(string? id, string action, string? addedBy, TileType tileType)
        {
            Id = id;
            Action = action;
            AddedBy = addedBy;
            TileType = tileType;
        }

        public Tile(MySqlDataReader reader)
        {
            Id = reader.GetString("Id");
            Action = reader.GetString("Action");
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string? AddedBy { get; }
        public TileType? TileType { get; }
    }

    public enum TileType
    {
        UserTile,
        PackTile
    }
}