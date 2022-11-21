using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardTileRepository : IBoardTileRepository
    {
        private readonly MySqlConnection _connection;

        public BoardTileRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }


        public async Task<BoardTile> ReadById(string id)
        {
            await using var con = _connection.Clone();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT BoardTile.Id AS BoardTile_Id, 
       Board_Id, Board_GameId, Board_UserId, 
       BoardTile.TileId AS ByTile_Id, Tile.Id AS Tile_Id, Tile.Action AS Tile_Action, 
       CASE WHEN PackTile.TileId IS NULL THEN '0' ELSE '1' END AS ByTile_Type, 
       User.id AS User_Id, User.username AS User_Username, User.nickname AS User_Nickname, User.ProfilePicURL AS User_ProfilePicUrl, 
       BoardTile.Position AS BoardTile_Position, BoardTile.IsActivated AS BoardTile_IsActivated 
FROM BoardTile 
    JOIN Board ON BoardTile.BoardId = Board.Board_Id
    JOIN User ON BoardTile.AboutUserId = User.id 
    LEFT JOIN PackTile ON BoardTile.TileId = PackTile.Id 
    LEFT JOIN TilePack ON PackTile.PackId = TilePack.Id 
    LEFT JOIN UserTile ON BoardTile.TileId = UserTile.Id 
    LEFT JOIN Tile ON PackTile.TileId = Tile.Id || UserTile.TileId = Tile.Id 
WHERE BoardTile.Id = @Id;",
                con);
            command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new BoardTile(reader);
            throw new Exception($"no {nameof(BoardTile)} with id: " + id);
        }

        public async Task<BoardTileEntity> Create(BoardTileEntity toCreate)
        {
            toCreate.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "INSERT INTO BoardTile(Id, AboutUserId, BoardId, TileId, Position, IsActivated) VALUES (@Id, @AboutUserId , @BoardId, @TileId, @Position, @IsActivated);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@AboutUserId", MySqlDbType.VarChar).Value = toCreate.AboutUserId;
                    command.Parameters.Add("@BoardId", MySqlDbType.VarChar).Value = toCreate.BoardId;
                    command.Parameters.Add("@TileId", MySqlDbType.VarChar).Value = toCreate.TileId;
                    command.Parameters.Add("@Position", MySqlDbType.Int16).Value = toCreate.Position;
                    command.Parameters.Add("@IsActivated", MySqlDbType.Bool).Value = toCreate.IsActivated;
                }
                command.ExecuteNonQuery();
            }
            return toCreate;
        }


        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            await using var con = _connection.Clone();
            List<BoardTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"SELECT BoardTile.Id AS BoardTile_Id, 
       Board_Id, Board_GameId, Board_UserId, 
       BoardTile.TileId AS ByTile_Id, Tile.Id AS Tile_Id, Tile.Action AS Tile_Action, 
       CASE WHEN PackTile.TileId IS NULL THEN '0' ELSE '1' END AS ByTile_Type, 
       User.id AS User_Id, User.username AS User_Username, User.nickname AS User_Nickname, User.ProfilePicURL AS User_ProfilePicUrl, 
       BoardTile.Position AS BoardTile_Position, BoardTile.IsActivated AS BoardTile_IsActivated 
FROM BoardTile 
    JOIN Board ON BoardTile.BoardId = Board.Board_Id 
    JOIN User ON BoardTile.AboutUserId = User.id 
    LEFT JOIN PackTile ON BoardTile.TileId = PackTile.Id 
    LEFT JOIN TilePack ON PackTile.PackId = TilePack.Id 
    LEFT JOIN UserTile ON BoardTile.TileId = UserTile.Id 
    LEFT JOIN Tile ON PackTile.TileId = Tile.Id || UserTile.TileId = Tile.Id 
WHERE BoardTile.BoardId = @boardId;",
                con);
            command.Parameters.Add("@boardId", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new BoardTile(reader));
            return list;
        }

        public async Task<BoardTileEntity> Update(BoardTileEntity toUpdate)
        {
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE BoardTile SET `AboutUserId`=@AboutUserId,`BoardId`=@BoardId,`TileId`=@TileId,`Position`=@Position,`IsActivated`=@IsActivated WHERE BoardTile.Id  = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toUpdate.Id;
                    command.Parameters.Add("@AboutUserId", MySqlDbType.VarChar).Value = toUpdate.AboutUserId;
                    command.Parameters.Add("@BoardId", MySqlDbType.VarChar).Value = toUpdate.BoardId;
                    command.Parameters.Add("@TileId", MySqlDbType.VarChar).Value = toUpdate.TileId;
                    command.Parameters.Add("@Position", MySqlDbType.Int16).Value = toUpdate.Position;
                    command.Parameters.Add("@IsActivated", MySqlDbType.Int16).Value =
                        Convert.ToByte(toUpdate.IsActivated);
                }
                await command.ExecuteNonQueryAsync();
            }
            return toUpdate;
        }
    }
}