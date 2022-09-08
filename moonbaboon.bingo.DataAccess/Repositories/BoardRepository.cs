using System;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly MySqlConnection _connection = new(DbStrings.SqlConnection);

        public async Task<Board> FindById(string id)
        {
            Board? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT * FROM `{DbStrings.BoardTable}` " +
                $"WHERE `{DbStrings.Id}`='{id}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = new Board(reader.GetValue(1).ToString(), reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString(),
                };
            }

            await _connection.CloseAsync();
            return ent ?? throw new Exception("no Board found with id: " + id);
        }

        public async Task<Board> Create(string userId, string gameId)
        {
            Board? ent = null;
            
            string uuid = Guid.NewGuid().ToString();

            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"INSERT INTO `{DbStrings.BoardTable}`(`{DbStrings.Id}`, `{DbStrings.GameId}`, `{DbStrings.UserId}`) " +
                $"VALUES ('{uuid}','{gameId}','{userId}'); " +
                $"SELECT * FROM `{DbStrings.BoardTable}` " +
                $"WHERE `{DbStrings.Id}`='{uuid}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = new Board(reader.GetValue(1).ToString(), reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString(),
                };
            }

            await _connection.CloseAsync();

            return ent ??
                   throw new Exception("Error in creating board for user: " + userId + " and game id: " + gameId);
        }

        public async Task<Board?> FindByUserAndGameId(string userId, string gameId)
        {
            Board? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT * FROM `{DbStrings.BoardTable}` " +
                $"WHERE `{DbStrings.UserId}`='{userId}' AND {DbStrings.BoardTable}.{DbStrings.GameId} = '{gameId}';",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = new Board(reader.GetValue(1).ToString(), reader.GetValue(2).ToString())
                {
                    Id = reader.GetValue(0).ToString(),
                };
            }

            await _connection.CloseAsync();
            return ent;
        }

        public async Task<bool> IsBoardFilled(string boardId)
        {
            var b = false;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT((SELECT COUNT(*) FROM {DbStrings.BoardTileTable} WHERE {DbStrings.BoardTileTable}.{DbStrings.IsActivated} = '1' AND {DbStrings.BoardTileTable}.{DbStrings.BoardId} ='{boardId}') = 24 IS true)",
                _connection);
            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                b = reader.GetBoolean(0);
            }

            await _connection.CloseAsync();
            return b;
        }
    }
}