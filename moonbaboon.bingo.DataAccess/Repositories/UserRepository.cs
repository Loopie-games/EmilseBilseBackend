using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly MySqlConnection _connection = new(DatabaseStrings.SqLconnection);
        
        public async Task<List<User>> FindAll()
        {
            var list = new List<User>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.UserTable}` ORDER BY `{DatabaseStrings.Id}`;", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToUser(reader);
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        private static User? ReaderToUser(MySqlDataReader reader)
        {
            var ent = new User(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), reader.GetValue(3).ToString(),
                reader.GetValue(4).ToString())
            {
                Id = reader.GetValue(0).ToString()
            };
            var ppUrl = reader.GetValue(5).ToString();
            if (!string.IsNullOrEmpty(ppUrl))
            {
                ent.ProfilePicUrl = ppUrl;
            }

            return ent;
        }

        public async Task<User?> Login(string dtoUsername, string dtoPassword)
        {
            User? user = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.UserTable}` WHERE `{DatabaseStrings.Username}` = '{dtoUsername}' AND `{DatabaseStrings.Password}` = '{dtoPassword}';", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                user = ReaderToUser(reader);
            }
            await _connection.CloseAsync();
            return user;
        }

        public async Task<User?> ReadById(string id)
        {
            User? user = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{DatabaseStrings.UserTable}` WHERE `{DatabaseStrings.Id}` = '{id}';", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                user = ReaderToUser(reader);
            }
            await _connection.CloseAsync();
            return user;
        }

        public async Task<User> Create(User user)
        {
            User? ent = null;
            string uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            var insertInto = $"INSERT INTO `{DatabaseStrings.UserTable}`(`{DatabaseStrings.Id}`, `{DatabaseStrings.Username}`, `{DatabaseStrings.Password}`, `{DatabaseStrings.Salt}`, `{DatabaseStrings.Nickname}`";
            var values = $"VALUES ('{uuid}','{user.Username}', '{user.Password}', '{user.Salt}', '{user.Nickname}'";
            
            if(!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                insertInto += $", `{DatabaseStrings.ProfilePic}`";
                values += $", '{user.ProfilePicUrl}'";
            }

            values += ");";
            insertInto += ") ";
            
            await using var command = new MySqlCommand(
                insertInto + values +
                $"SELECT * FROM `{DatabaseStrings.UserTable}` " +
                $"WHERE `{DatabaseStrings.Id}` = '{uuid}'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString() == user.Username)
                {
                    ent = ReaderToUser(reader);
                }
            }
            
            await _connection.CloseAsync();

            if (ent == null)
            {
                throw new InvalidDataException("ERROR: User not created");
            }
            return ent;
        }

        public async Task<bool> VerifyUsername(string username)
        {
            username = username.ToLower();
            var b = true;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT * FROM `{DatabaseStrings.UserTable}` " +
                $"WHERE Lower(`{DatabaseStrings.Username}`) = '{username}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString()?.ToLower() == username)
                {
                    b = false;
                }
            }
            
            await _connection.CloseAsync();
            return b;
        }

        public async Task<string?> GetSalt(string username)
        {
            string? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT {DatabaseStrings.UserTable}.{DatabaseStrings.Salt} FROM `{DatabaseStrings.UserTable}` WHERE `{DatabaseStrings.Username}` = '{username}'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = reader.GetValue(0).ToString();
            }
            await _connection.CloseAsync();
            return ent ?? "null";
        }

        public async Task<User?> GetByUsername(string username)
        {
            User? ent = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT * FROM `{DatabaseStrings.UserTable}` " +
                $"WHERE `{DatabaseStrings.Username}` = '{username}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                ent = ReaderToUser(reader);
            }
            
            await _connection.CloseAsync();
            return ent;
        }
    }
}