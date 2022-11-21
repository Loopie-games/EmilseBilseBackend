using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameModeRepository : IGameModeRepository
    {
        private readonly MySqlConnection _connection;

        public GameModeRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }

        public async Task<List<GameMode>> FindAll()
        {
            List<GameMode> list = new();
            await using var con = _connection.Clone();
            {
                con.Open();

                await using MySqlCommand command =
                    new(
                        @"SELECT GameMode.Id AS GameMode_Id, GameMode.Name AS GameMode_Name FROM GameMode;",
                        con);
                

                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) list.Add(new GameMode(reader));
            }
            return list;
        }
    }
}