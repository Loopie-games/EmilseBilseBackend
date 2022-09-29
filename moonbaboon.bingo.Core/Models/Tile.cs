using System.Data;
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
            Id = reader.GetString("Tile_Id");
            Action = reader.GetString("Tile_Action");
        }

        public string? Id { get; set; }
        public string Action { get; set; }
        public string? AddedBy { get; set; }
        public TileType? TileType { get; set; }
    }

    public enum TileType
    {
        UserTile,
        PackTile
    }
}