using System;
using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class ByTile
    {
        public ByTile(string? id, Tile tile, TileType? tileType)
        {
            Id = id;
            Tile = tile;
            TileType = tileType;
        }

        public ByTile(MySqlDataReader reader)
        {
            Id = reader.GetString("ByTile_Id");
            Tile = new Tile(reader);
            TileType = Enum.Parse<TileType>(reader.GetValue("ByTile_Type").ToString());
        }

        public string? Id { get; set; }
        public Tile Tile { get; set; }

        public TileType? TileType { get; }
    }

    public enum TileType
    {
        UserTile,
        PackTile
    }
}