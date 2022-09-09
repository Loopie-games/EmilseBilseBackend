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
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);
        private const string Table = DbStrings.BoardTileTable;

        private static string sql_select(string from)
        {
            return
                $"SELECT {Table}.{DbStrings.Id}, {Table}.{DbStrings.Position}, {Table}.{DbStrings.IsActivated}, " +
                $"{DbStrings.BoardTable}.{DbStrings.Id}, {DbStrings.BoardTable}.{DbStrings.GameId}, {DbStrings.BoardTable}.{DbStrings.UserId}, " +
                $"{DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic}, " +
                $"{DbStrings.TileTable}.{DbStrings.Id}, {DbStrings.TileTable}.{DbStrings.Action} " +
                $"FROM {from} " +
                $"JOIN {DbStrings.BoardTable} ON {Table}.{DbStrings.BoardId} = {DbStrings.BoardTable}.{DbStrings.Id} " +
                $"JOIN {DbStrings.UserTable} on {Table}.{DbStrings.AboutUserId} = {DbStrings.UserTable}.{DbStrings.Id} " +
                $"JOIN {DbStrings.TileTable} ON {Table}.{DbStrings.TileId} = {DbStrings.TileTable}.{DbStrings.Id} ";
        }

        private static BoardTile ReaderToBoardTile(MySqlDataReader reader)
        {
            Board board =
                new(reader.GetString(3), reader.GetString(4), reader.GetString(5));
            UserSimple user =
                new(reader.GetString(6), reader.GetString(7), reader.GetString(8), 
                    reader.GetValue(9).ToString());
            Tile tile =
                new(reader.GetString(10), reader.GetString(11));
            BoardTile boardTile =
                new(reader.GetString(0), board, tile, user, reader.GetInt32(1),
                    Convert.ToBoolean(reader.GetValue(2).ToString()));

            return boardTile;
        }

        public async Task<BoardTile> ReadById(string id)
        {
            BoardTile? ent = null;

            await _connection.OpenAsync();
            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);
            }

            await _connection.CloseAsync();

            return ent ?? throw new Exception($"no {Table} found with id: " + id);
        }

        public async Task<BoardTile> Create(BoardTile toCreate)
        {
            string uuid = Guid.NewGuid().ToString();
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO {Table} " +
                $"VALUES ('{uuid}','{toCreate.AboutUser.Id}','{toCreate.Board.Id}','{toCreate.Tile.Id}','{toCreate.Position}','{Convert.ToInt32(toCreate.IsActivated)}'); " +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{uuid}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);
            }

            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR: {Table} not created");
            }

            return ent;
        }

        public async Task<List<BoardTile>> FindByBoardId(string id)
        {
            List<BoardTile> list = new();
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.BoardId} = '{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                BoardTile boardTile = ReaderToBoardTile(reader);
                list.Add(boardTile);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<BoardTile> Update(BoardTile toUpdate)
        {
            BoardTile? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"UPDATE {Table} " +
                $"SET `{DbStrings.AboutUserId}`='{toUpdate.AboutUser.Id}',`{DbStrings.BoardId}`='{toUpdate.Board.Id}',`{DbStrings.TileId}`='{toUpdate.Tile.Id}',`{DbStrings.Position}`='{toUpdate.Position}',`{DbStrings.IsActivated}`='{Convert.ToInt32(toUpdate.IsActivated)}' " +
                $"WHERE {DbStrings.Id} = '{toUpdate.Id}';" +
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} = '{toUpdate.Id}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToBoardTile(reader);
            }

            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException($"ERROR: {Table} with id {toUpdate.Id} not updated");
            }

            return ent;
        }
    }
}