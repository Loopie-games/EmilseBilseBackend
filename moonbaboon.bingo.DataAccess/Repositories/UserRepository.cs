using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string Table = DbStrings.UserTable;
        private readonly MySqlConnection _connection;
        private readonly Random _random = new();

        public UserRepository(MySqlConnection connection)
        {
            _connection = connection.Clone();
        }
        
        public async Task<List<User>> GetPlayers(string gameId)
        {
            var list = new List<User>();
            await using var con = _connection.Clone();
            con.Open();

            await using var command = new MySqlCommand(
                @"SELECT * From (SELECT Board_UserId as u1 FROM `Game` JOIN Board ON Board_GameId = @GameId) As b JOIN User ON b.u1 = User.User_id",
                con);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new User(reader));
            }

            return list;
        }

        public async Task<List<User>> Search(string searchString)
        {
            var list = new List<User>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Username} " +
                $"LIKE '%{searchString}%'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<List<User>> SearchID(string searchString)
        {
            var list = new List<User>();
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                sql_select(Table) +
                $"WHERE {Table}.{DbStrings.Id} " +
                $"LIKE '%{searchString}%'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var ent = ReaderToEnt(reader);
                list.Add(ent);
            }

            await _connection.CloseAsync();
            return list;
        }

        public async Task<User> Login(string dtoUsername, string dtoPassword)
        {
            User? user = null;
            await _connection.OpenAsync();

            await using var command =
                new MySqlCommand(
                    sql_select(Table) +
                    $"WHERE `{DbStrings.Username}` = '{dtoUsername}' AND `{DbStrings.Password}` = '{dtoPassword}';",
                    _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) user = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return user ?? throw new InvalidOperationException("Invalid Login");
        }

        /// <summary>
        ///     Finds User with specified id
        /// </summary>
        /// <param name="id">unique user Identification</param>
        /// <returns>A Task containing the user</returns>
        /// <exception cref="Exception">No user with given id</exception>
        public async Task<User> ReadById(string id)
        {
            User? user = null;
            await _connection.OpenAsync();

            await using var command =
                new MySqlCommand(sql_select(Table) +
                                 $"WHERE `{DbStrings.Id}` = '{id}';",
                    _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) user = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return user ?? throw new Exception("No user found with id: " + id);
        }

        public async Task<User> Create(User user)
        {
            User? ent = null;
            string uuid = Guid.NewGuid().ToString();
            await _connection.OpenAsync();

            var insertInto =
                $"INSERT INTO `{DbStrings.UserTable}`(`{DbStrings.Id}`, `{DbStrings.Username}`, `{DbStrings.Password}`, `{DbStrings.Salt}`, `{DbStrings.Nickname}`";
            var values = $"VALUES ('{uuid}','{user.Username}', '{user.Password}', '{user.Salt}', '{user.Nickname}'";

            if (!string.IsNullOrEmpty(user.ProfilePicUrl))
            {
                insertInto += $", `{DbStrings.ProfilePic}`";
                values += $", '{user.ProfilePicUrl}'";
            }

            values += ");";
            insertInto += ") ";

            await using var command = new MySqlCommand(
                insertInto + values +
                sql_select(Table) +
                $"WHERE `{DbStrings.Id}` = '{uuid}'", _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = ReaderToEnt(reader);

            await _connection.CloseAsync();
            return ent ?? throw new InvalidDataException("ERROR: User not created");
        }

        public async Task<bool> VerifyUsername(string username)
        {
            username = username.ToLower();
            var b = true;
            await _connection.OpenAsync();

            await using var command = new MySqlCommand(
                $"SELECT * FROM `{DbStrings.UserTable}` " +
                $"WHERE Lower(`{DbStrings.Username}`) = '{username}';",
                _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                if (reader.GetValue(1).ToString()?.ToLower() == username)
                    b = false;

            await _connection.CloseAsync();
            return b;
        }

        public async Task<string> GetSalt(string username)
        {
            string? ent = null;
            await _connection.OpenAsync();

            await using var command =
                new MySqlCommand(
                    $"SELECT {DbStrings.UserTable}.{DbStrings.Salt} FROM `{DbStrings.UserTable}` WHERE `{DbStrings.Username}` = '{username}'",
                    _connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ent = reader.GetValue(0).ToString();

            await _connection.CloseAsync();
            return ent ?? throw new Exception("no user with given username");
        }

        private static string sql_select(string from)
        {
            return
                $"SELECT {DbStrings.UserTable}.{DbStrings.Id}, {DbStrings.UserTable}.{DbStrings.Username}, {DbStrings.UserTable}.{DbStrings.Nickname}, {DbStrings.UserTable}.{DbStrings.ProfilePic} " +
                $"FROM {from} ";
        }

        private static User ReaderToEnt(MySqlDataReader reader)
        {
            var ent =
                new User(reader.GetString(0), reader.GetString(1),
                    reader.GetString(2), reader.GetValue(3).ToString());
            return ent;
        }

        public async Task<User> UpdateUser(string id, User user)
        {

            await _connection.OpenAsync();

            string query = $"UPDATE {DbStrings.UserTable} SET " +
                            $"{DbStrings.Username} = '{user.Username}', " +
                            $"{DbStrings.Nickname} = '{user.Nickname}', " +
                            $"{DbStrings.ProfilePic} = '{user.ProfilePicUrl}' " +
                            $"WHERE {DbStrings.Id} = '{user.Id}'";

            await using var command = new MySqlCommand(query,
                _connection);

            await using var reader = await command.ExecuteReaderAsync();
            await _connection.CloseAsync();

            return new User(user.Id, user.Username, user.Nickname, user.ProfilePicUrl);
        }

        public async Task<bool> RemoveBanner(string uuid)
        {
            await _connection.OpenAsync();

            await _connection.CloseAsync();
            return false;
        }

        public async Task<bool> RemoveIcon(string uuid)
        {
            await _connection.OpenAsync();

            await _connection.CloseAsync();
            return false;
        }

        public async Task<bool> RemoveName(string uuid)
        {
            await _connection.OpenAsync();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newRandomName = new(Enumerable.Repeat(chars, 16)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            string query = $"UPDATE {DbStrings.UserTable} SET " +
                            $"{DbStrings.Username} = '{newRandomName}'" +
                            $"WHERE {DbStrings.Id} = '{uuid}'";

            await using var command = new MySqlCommand(query, _connection);

            int affectedRows = command.ExecuteNonQuery();

            await _connection.CloseAsync();

            return (affectedRows > 0);
        }
    }
}