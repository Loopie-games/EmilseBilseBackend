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
            con.Open();

            await using MySqlCommand command = new(
                @"SELECT BoardTile.*, Board.*, Tile.*, U.User_id As ActivatedBy, U.*, U2.User_id As AboutUser, U2.*
                FROM BoardTile 
                    JOIN Board ON BoardTile_BoardId = Board.Board_Id 
                    LEFT JOIN User U on U.User_id = BoardTile.BoardTile_ActivatedBy
                    LEFT JOIN User U2 on U2.User_id = BoardTile.BoardTile_AboutUserId
                    LEFT JOIN Tile ON BoardTile_TileId = Tile.Tile_Id 
                WHERE BoardTile_Id = @Id;",
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
                        "INSERT INTO BoardTile VALUES (@Id, @AboutUserId , @BoardId, @TileId, @Position, @IsActivated);",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toCreate.Id;
                    command.Parameters.Add("@AboutUserId", MySqlDbType.VarChar).Value = toCreate.AboutUserId;
                    command.Parameters.Add("@BoardId", MySqlDbType.VarChar).Value = toCreate.BoardId;
                    command.Parameters.Add("@TileId", MySqlDbType.VarChar).Value = toCreate.TileId;
                    command.Parameters.Add("@Position", MySqlDbType.Int16).Value = toCreate.Position;
                    command.Parameters.Add("@IsActivated", MySqlDbType.VarChar).Value = toCreate.ActivatedBy;
                }
                command.ExecuteNonQuery();
            }
            return toCreate;
        }


        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            await using var con = _connection.Clone();
            List<BoardTile> list = new();
            con.Open();

            await using MySqlCommand command = new(
                @"SELECT BoardTile.*, Board.*, Tile.*, U.User_id As ActivatedBy, U.*, U2.User_id As AboutUser, U2.*
                FROM BoardTile 
                    JOIN Board ON BoardTile_BoardId = Board.Board_Id 
                    LEFT JOIN User U on U.User_id = BoardTile.BoardTile_ActivatedBy
                    LEFT JOIN User U2 on U2.User_id = BoardTile.BoardTile_AboutUserId
                    LEFT JOIN Tile ON BoardTile_TileId = Tile.Tile_Id
                WHERE BoardTile_BoardId = @boardId
ORDER BY BoardTile_Position                 ",
                con);
            command.Parameters.Add("@boardId", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new BoardTile(reader));
            }

            return list;
        }

        public async Task<BoardTileEntity> Update(BoardTileEntity toUpdate)
        {
            await using var con = _connection.Clone();
            {
                con.Open();
                await using MySqlCommand command =
                    new(
                        "UPDATE BoardTile SET BoardTile_AboutUserId=@AboutUserId, BoardTile_BoardId=@BoardId,BoardTile_TileId=@TileId,BoardTile_Position=@Position,BoardTile_ActivatedBy=@IsActivated WHERE BoardTile_Id  = @Id",
                        con);
                {
                    command.Parameters.Add("@Id", MySqlDbType.VarChar).Value = toUpdate.Id;
                    command.Parameters.Add("@AboutUserId", MySqlDbType.VarChar).Value = toUpdate.AboutUserId;
                    command.Parameters.Add("@BoardId", MySqlDbType.VarChar).Value = toUpdate.BoardId;
                    command.Parameters.Add("@TileId", MySqlDbType.VarChar).Value = toUpdate.TileId;
                    command.Parameters.Add("@Position", MySqlDbType.Int16).Value = toUpdate.Position;
                    command.Parameters.Add("@IsActivated", MySqlDbType.VarChar).Value = toUpdate.ActivatedBy;
                }
                await command.ExecuteNonQueryAsync();
            }
            return toUpdate;
        }
    }
}