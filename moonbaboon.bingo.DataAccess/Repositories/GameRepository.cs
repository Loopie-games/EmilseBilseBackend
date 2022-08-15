using System;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class GameRepository: IGameRepository
    {
        private readonly MySqlConnection _connection = new(DBStrings.SqLconnection);
        private static readonly Random Random = new();
        
        public async Task<Game?> FindById(string id)
        {   
            Game? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT {DBStrings.GameTable}.{DBStrings.Id}, " +
                $"{DBStrings.UserTable}.{DBStrings.Id}, {DBStrings.UserTable}.{DBStrings.Username}, {DBStrings.UserTable}.{DBStrings.Nickname}, {DBStrings.UserTable}.{DBStrings.ProfilePic}, " +
                $"{DBStrings.GameTable}.{DBStrings.WinnerId} " +
                $"FROM `{DBStrings.GameTable}` " +
                $"JOIN {DBStrings.UserTable} ON {DBStrings.UserTable}.{DBStrings.Id} = {DBStrings.GameTable}.{DBStrings.HostId} " +
                $"WHERE {DBStrings.GameTable}.{DBStrings.Id} = '{id}'", 
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var host = new UserSimple(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                ent = new Game(host)
                {
                    Id = reader.GetValue(0).ToString(),
                    WinnerId = reader.GetValue(5).ToString(),
                };
            }
            await _connection.CloseAsync();
            return ent;
        }
    }
}