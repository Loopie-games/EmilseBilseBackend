using System.Collections.Generic;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserRepository: IUserRepository
    {
        //Table
        private const string Table = "User";
        
        //Rows
        private const string Id = "id";
        private const string Username = "username";
        private const string Password = "password";
        private const string Nickname = "nickname";
        
        private readonly MySqlConnection _connection = new MySqlConnection("Server=185.51.76.204; Database=emilse_bilse_bingo; Uid=root; PWD=hemmeligt;");
        
        public async Task<List<User>> FindAll()
        {
            var list = new List<User>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` ORDER BY `{Id}`;", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = new User(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(),reader.GetValue(2).ToString(),reader.GetValue(3).ToString());
                list.Add(ent);
                
            }
            await _connection.CloseAsync();
            return list;
        }
    }
}