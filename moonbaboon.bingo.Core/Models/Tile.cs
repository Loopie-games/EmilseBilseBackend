

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public Tile(string? id, string action)
        {
            Id = id;
            Action = action;
            AddedBy = null;
            TileType = null;
        }

        public Tile(MySqlDataReader reader, IReadOnlyList<string>? names)
        {
            Id = reader.GetString(names?[0] ?? "Id");
            Action = reader.GetString(names?[1] ?? "Action");
            AddedBy = null;
            TileType = null;
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