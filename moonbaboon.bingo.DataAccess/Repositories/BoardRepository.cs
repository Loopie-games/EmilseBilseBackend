using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private const string Table = DbStrings.BoardTable;
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<Board> FindById(string id)
        {
            Board? ent = null;

            await _connection.OpenAsync();
            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE `{DbStrings.Id}`='{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();

            return ent ?? throw new Exception($"no {Table} found with id: " + id);
        }

        public async Task<Board> Create(string userId, string gameId)
        {
            Board? ent = null;
            string uuid = Guid.NewGuid().ToString();

            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO {Table} (`{DbStrings.Id}`, `{DbStrings.GameId}`, `{DbStrings.UserId}`) " +
                $"VALUES ('{uuid}','{gameId}','{userId}'); " +
                sql_select(Table) +
                $"WHERE `{DbStrings.Id}`='{uuid}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();

            return ent ??
                   throw new Exception("Error in creating board for user: " + userId + " and game id: " + gameId);
        }

        public async Task<Board?> FindByUserAndGameId(string userId, string gameId)
        {
            Board? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                sql_select(Table) +
                $"WHERE `{DbStrings.UserId}`='{userId}' AND {DbStrings.BoardTable}.{DbStrings.GameId} = '{gameId}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();

            return ent;
        }

        public async Task<bool> IsBoardFilled(string boardId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                "SELECT((SELECT COUNT(*) " +
                $"FROM {DbStrings.BoardTileTable} " +
                $"WHERE {DbStrings.BoardTileTable}.{DbStrings.IsActivated} = '1' " +
                $"AND {DbStrings.BoardTileTable}.{DbStrings.BoardId} ='{boardId}') " +
                "= 24 IS true)",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) b = reader.GetBoolean(0);

            await _connection.CloseAsync();
            return b;
        }

        public async Task<List<Board>> FindTopRanking(string gameId, int limit)
        {
            var list = new List<Board>();
            await using var con = new MySqlConnection(DbStrings.SqlConnection);
            con.Open();

            string sqlCommand =
                sql_select(Table) +
                $"WHERE Board.GameId = '{gameId}' " +
                "ORDER BY TT DESC " +
                $"Limit {limit};";

            await using var command = new MySqlCommand(sqlCommand, con);
            await using var reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }

            return list;
        }

        private static string sql_select(string from)
        {
            return
                "SELECT *, ( SELECT COUNT(BT.Id) FROM BoardTile AS BT WHERE BT.IsActivated = '1' && BT.BoardId = Board.Id ) AS TT " +
                $"FROM {from} ";
        }

        private Board ReaderToEnt(MySqlDataReader reader)
        {
            return new Board(reader.GetString(0), reader.GetString(1), reader.GetString(2))
            {
                TurnedTiles = reader.GetInt32(3)
            };
        }
    }
}