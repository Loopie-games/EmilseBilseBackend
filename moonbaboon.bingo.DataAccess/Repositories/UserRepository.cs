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
                var ent = new User(reader.GetValue(1).ToString(),reader.GetValue(2).ToString(),reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(0).ToString()
                };
                list.Add(ent);
            }
            await _connection.CloseAsync();
            return list;
        }

        public async Task<User?> Login(string dtoUsername, string dtoPassword)
        {
            User? user = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE `{Username}` = '{dtoUsername}' AND `{Password}` = '{dtoPassword}';", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                user = new User(reader.GetValue(1).ToString(),reader.GetValue(2).ToString(),reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(0).ToString()
                };
            }
            await _connection.CloseAsync();
            return user;
        }

        public async Task<User?> ReadById(string id)
        {
            
            User? user = null;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE `{Id}` = '{id}';", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                user = new User(reader.GetValue(1).ToString(),reader.GetValue(2).ToString(),reader.GetValue(3).ToString())
                {
                    Id = reader.GetValue(0).ToString()
                };
            }
            await _connection.CloseAsync();
            return user;
        }

        public async Task<User> Create(User user)
        {
            User? ent = null;
            string uuid = System.Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand($"INSERT INTO `{Table}`(`{Id}`, `{Username}`, `{Password}`, `{Nickname}`) VALUES ('{uuid}','{user.Username}', '{user.Password}', '{user.Nickname}'); SELECT * FROM `{Table}` WHERE `{Id}` = '{uuid}'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetValue(1).ToString() == user.Username)
                {
                    ent = new User(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                        reader.GetValue(3).ToString())
                    {
                        Id = reader.GetValue(0).ToString()
                    };
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

            await using var command = new MySqlCommand($"SELECT * FROM `{Table}` WHERE Lower(`{Username}`) = '{username}';", _connection);
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
    }
}