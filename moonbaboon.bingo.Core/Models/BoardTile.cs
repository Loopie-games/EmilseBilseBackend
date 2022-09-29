using System;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardTile
    {
        public BoardTile(string? id, Board board, ByTile byTile, UserSimple aboutUser, int position, bool isActivated)
        {
            Id = id;
            Board = board;
            ByTile = byTile;
            AboutUser = aboutUser;
            Position = position;
            IsActivated = isActivated;
        }

        public BoardTile(MySqlDataReader reader)
        {
            Id = reader.GetString("BoardTile_Id");
            Board = new Board(reader);
            ByTile = new ByTile(reader);
            AboutUser = new UserSimple(reader);
            Position = reader.GetInt32("BoardTile_Position");
            IsActivated = Convert.ToBoolean(reader.GetInt32("BoardTile_IsActivated"));

        }

        public string? Id { get; set; }
        public Board Board { get; set; }
        public ByTile ByTile { get; set; }
        public UserSimple AboutUser { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }

    public class BoardTileEntity
    {
        public BoardTileEntity(string id, string aboutUserId, string boardId, string tileId, int position, bool isActivated)
        {
            Id = id;
            AboutUserId = aboutUserId;
            BoardId = boardId;
            TileId = tileId;
            Position = position;
            IsActivated = isActivated;
        }

        public BoardTileEntity(BoardTile boardTile)
        {
            Id = boardTile.Id;
            AboutUserId = boardTile.Id;
            BoardId = boardTile.Board.Id;
            TileId = boardTile.ByTile.Id;
            Position = boardTile.Position;
            IsActivated = boardTile.IsActivated;
        }

        public string? Id { get; set; }
        public string AboutUserId { get; set; }
        public string BoardId { get; set; }
        public string TileId { get; set; }
        public int Position { get; set; }
        public bool IsActivated { get; set; }
    }
}