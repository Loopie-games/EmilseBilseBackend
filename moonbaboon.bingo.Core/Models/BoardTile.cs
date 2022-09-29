using System;
using System.Data;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardTile
    {
        public BoardTile(string? id, Board board, Tile tile, UserSimple aboutUser, int position, bool isActivated)
        {
            Id = id;
            Board = board;
            Tile = tile;
            AboutUser = aboutUser;
            Position = position;
            IsActivated = isActivated;
        }

        public BoardTile(MySqlDataReader reader)
        {
            Id = reader.GetString("BoardTile_Id");
            Board = new Board(reader);
            Tile = new Tile(reader)
            {
                AddedBy = reader.GetString("BoardTile_AddedBy")
            };
            if (Enum.Parse<TileType>(reader.GetString("BoardTile_TileType")) == TileType.PackTile)
            {
                Tile.TileType = TileType.PackTile;
            }
            else if(Enum.Parse<TileType>(reader.GetString("BoardTile_TileType")) == TileType.UserTile)
            {
                Tile.TileType = TileType.UserTile;
            }
            AboutUser = new UserSimple(reader);
            Position = reader.GetInt32("BoardTile_Position");
            IsActivated = reader.GetBoolean("BoardTile_IsActivated");
        }

        public string? Id { get; set; }
        public Board Board { get; set; }
        public Tile Tile { get; set; }
        public UserSimple AboutUser { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}