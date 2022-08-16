using System;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class BoardRepository :IBoardRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        private static readonly Random Random = new();

        public async Task<Board?> FindById(string id)
        {
            Board? ent = null;
            await _connection.OpenAsync();

            await using MySqlCommand command = new(
                $"SELECT * FROM `{DBStrings.BoardTable}` " +
                $"WHERE `{DBStrings.Id}`='{id}';", 
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
    }
}