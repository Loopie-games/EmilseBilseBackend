using System;
using MySqlConnector;

namespace moonbaboon.bingo.Core.Models
{
    public class BoardTile
    {
        public BoardTile(MySqlDataReader reader)
        {
            Id = reader.GetString("BoardTile_Id");
            BoardEntity = new BoardEntity(reader);
            Tile = string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("Tile_Id")).ToString())
                ? null
                : new Tile(reader);
            AboutUser = string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("AboutUser")).ToString())
                ? null
                : new User(reader, reader.GetOrdinal("AboutUser")+1);
            Position = reader.GetInt32("BoardTile_Position");
            ActivatedBy  = string.IsNullOrEmpty(reader.GetValue(reader.GetOrdinal("ActivatedBy")).ToString())
                ? null
                : new User(reader, reader.GetOrdinal("ActivatedBy")+1);
        }

        public string? Id { get; set; }
        public BoardEntity BoardEntity { get; set; }
        public Tile? Tile { get; set; }
        public User? AboutUser { get; set; }
        public int Position { get; set; }
        public User? ActivatedBy { get; set; }
    }

    public class BoardTileEntity
    {
        public BoardTileEntity(string? id, string? aboutUserId, string boardId, string? tileId, int position,
            string? activatedBy)
        {
            Id = id;
            AboutUserId = aboutUserId;
            BoardId = boardId;
            TileId = tileId;
            Position = position;
            ActivatedBy = activatedBy;
        }

        public BoardTileEntity(BoardTile boardTile)
        {
            Id = boardTile.Id;
            AboutUserId = boardTile.AboutUser?.Id;
            BoardId = boardTile.BoardEntity.Id;
            TileId = boardTile.Tile?.Id;
            Position = boardTile.Position;
            ActivatedBy = boardTile.ActivatedBy?.Id;
        }

        public string? Id { get; set; }
        public string? AboutUserId { get; set; }
        public string BoardId { get; set; }
        public string? TileId { get; set; }
        public int Position { get; set; }
        public string? ActivatedBy { get; set; }
    }
}