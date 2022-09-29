using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardTileRepository : IBoardTileRepository
    {
        private const string Table = DbStrings.BoardTileTable;
        private readonly MySqlConnection _connection;

        public BoardTileRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<BoardTile> ReadById(string id)
        {
            BoardTile? ent = null;
            await using var con = _connection.Clone();
            
            await con.OpenAsync();
            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{id}';",
                con);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToBoardTile(reader);
            return ent ?? throw new Exception($"no {Table} found with id: " + id);
        }

        public async Task<BoardTile> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            await using var command = new MySqlCommand(
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.AboutUser.Id}','{toCreate.Board.Id}', '{toCreate.Tile.TileType}','{toCreate.Tile.Id}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}';",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read()) ent = ReaderToBoardTile(reader);

            if (ent == null) throw new InvalidDataException($"ERROR: {Table} not created");

            return ent;
        }

        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            await using var con = _connection.Clone();
            List<BoardTile> list = new();
            await con.OpenAsync();

            await using MySqlCommand command = new(
                @"
SELECT BoardTile.Id As BoardTile_Id, BoardTile.TileType As BoardTile_TileType, BoardTile.Position As BoardTile_Position, BoardTile.IsActivated As BoardTile_IsActivated, 
       Board.Id AS Board_Id, Board.GameId AS Board_GameId, Board.UserId AS Board_UserId, 
       Tile.Id As Tile_Id, Tile.Action As Tile_Action, 
       AboutUser.id As User_Id, AboutUser.username AS User_Username, AboutUser.nickname AS User_Nickname, AboutUser.ProfilePicURL AS User_ProfilePicUrl, 
       CASE BoardTile.TileType WHEN 'UserTile' THEN ByUser.username WHEN 'PackTile' THEN TilePack.Name END As BoardTile_AddedBy 
FROM `BoardTile` JOIN Board On BoardTile.BoardId = Board.Id JOIN User As AboutUser ON BoardTile.AboutUserId = AboutUser.id 
    JOIN Tile ON BoardTile.TileId = Tile.Id 
    LEFT JOIN PackTile ON BoardTile.TileId = PackTile.TileId 
    LEFT JOIN TilePack ON TilePack.Id = PackTile.PackId 
    LEFT JOIN UserTile ON BoardTile.TileId= UserTile.TileId 
    LEFT JOIN User AS ByUser ON UserTile.AddedById = ByUser.id 
WHERE `BoardId` = @boardId;",
                con);
            command.Parameters.Add("@boardId", MySqlDbType.VarChar).Value = id;
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            { 
                list.Add(new BoardTile(reader));
            }

            await con.CloseAsync();
            return list;
        }
        
        public async Task<BoardTile> Update(BoardTile toUpdate)
        {
            await using var connection = new MySqlConnection(DbStrings.SqlConnection);
            BoardTile? ent = null;
            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {Table} " +
                $"SET `{DbStrings.AboutUserId}`='{toUpdate.AboutUser.Id}',`{DbStrings.BoardId}`='{toUpdate.Board.Id}',`{DbStrings.TileId}`='{toUpdate.Tile.Id}',`{DbStrings.Position}`='{toUpdate.Position}',`{DbStrings.IsActivated}`='{Convert.ToInt32(toUpdate.IsActivated)}' " +
                $"WHERE {DbStrings.Id} = '{toUpdate.Id}';" +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{toUpdate.Id}';",
                connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToBoardTile(reader);

            await connection.CloseAsync();

            if (ent == null) throw new InvalidDataException($"ERROR: {Table} with id {toUpdate.Id} not updated");

            return ent;
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DbStrings.Id}, {Table}.{DbStrings.Position}, {Table}.{DbStrings.IsActivated}, " +
                $"{DbStrings.BoardTable}.{DbStrings.Id}, {DbStrings.BoardTable}.{DbStrings.GameId}, {DbStrings.BoardTable}.{DbStrings.UserId}, " +
                $"{DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, " +
                $"TOM.Id, TOM.Action, TOM.AddedBy, {Table}.{DbStrings.TileType} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.BoardTable} ON {Table}.{DbStrings.BoardId} = {DbStrings.BoardTable}.{DbStrings.Id} " +
                $"JOIN {DbStrings.UserTable} on {Table}.{DbStrings.AboutUserId} = {DbStrings.UserTable}.{DbStrings.Id} " +
                "JOIN (" +
                "SELECT Tile.Id As Id, Tile.Action As Action, TilePack.Name AS AddedBy " +
                "FROM Tile JOIN PackTile ON Tile.Id = PackTile.TileId " +
                "LEFT JOIN TilePack ON TilePack.Id = PackTile.PackId " +
                "UNION SELECT Tile.Id, Tile.Action, User.username AS AddedBy " +
                "FROM Tile JOIN UserTile ON Tile.Id = UserTile.TileId LEFT " +
                "JOIN User ON UserTile.AddedById = User.id) AS TOM " +
                $"ON {Table}.{DbStrings.TileId} = TOM.ID ";
        }

        private static BoardTile ReaderToBoardTile(MySqlDataReader reader)
        {
            Board board =
                new(reader.GetString(3), reader.GetString(4), reader.GetString(5));
            UserSimple user =
                new(reader.GetString(6), reader.GetString(7), reader.GetString(8),
                    reader.GetValue(9).ToString());
            Tile tile = new(reader.GetString(10), reader.GetString(11), reader.GetString(12),
                Enum.Parse<TileType>(reader.GetValue(13).ToString()));
            BoardTile boardTile =
                new(reader.GetString(0), board, tile, user, reader.GetInt32(1),
                    Convert.ToBoolean(reader.GetValue(2).ToString()));

            return boardTile;
        }
    }
}