using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using moonbaboon.bingo.Core.Models;
using moonbaboon.bingo.Domain.IRepositories;
using MySqlConnector;

namespace moonbaboon.bingo.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _connection;
        private readonly Random _random = new();

        public UserRepository(MySqlConnection connection)
        {
            _connection = connection;
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
            while (await reader.ReadAsync()) list.Add(new User(reader));

            return list;
        }

        public async Task<List<User>> Search(string searchString)
        {
            var list = new List<User>();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"Select * from User WHERE User.User_Username LIKE @searchString", _connection);
            {
                command.Parameters.Add("@searchString", MySqlDbType.VarChar).Value = searchString;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) list.Add(new User(reader));

            return list;
        }

        public async Task<User> Login(string dtoUsername, string dtoPassword)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command =
                new MySqlCommand(
                    @"Select * from User 
JOIN Auth A on User.User_id = A.Auth_UserId
WHERE User_Username = @username AND  Auth_Password = @password",
                    con);
            {
                command.Parameters.Add("@username", MySqlDbType.VarChar).Value = dtoUsername;
                command.Parameters.Add("@password", MySqlDbType.VarChar).Value = dtoPassword;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new User(reader);

            throw new InvalidOperationException("Invalid Login");
        }

        /// <summary>
        ///     Finds User with specified id
        /// </summary>
        /// <param name="id">unique user Identification</param>
        /// <returns>A Task containing the user</returns>
        /// <exception cref="Exception">No user with given id</exception>
        public async Task<User> ReadById(string id)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command =
                new MySqlCommand(@"select * from User WHERE User_id = @id;",
                    con);
            {
                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = id;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return new User(reader);

            throw new Exception("No user found with id: " + id);
        }

        public async Task<string> Create(User entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"Insert into User VALUES (@id, @username, @nickname, @profilePic)", con);
            {
                command.Parameters.Add("@id", MySqlDbType.VarChar).Value = entity.Id;
                command.Parameters.Add("@username", MySqlDbType.VarChar).Value = entity.Username;
                command.Parameters.Add("@nickname", MySqlDbType.VarChar).Value = entity.Nickname;
                command.Parameters.Add("@profilePic", MySqlDbType.VarChar).Value = entity.ProfilePicUrl;
            }
            command.ExecuteNonQuery();
            return entity.Id;
        }

        public async Task<string?> GetUserIdByUsername(string username)
        {
            username = username.ToLower();
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"SELECT User_id FROM User WHERE Lower(User_Username) = @username",
                con);
            {
                command.Parameters.Add("@username", MySqlDbType.VarChar).Value = username.ToLower();
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return reader.GetString(0);
            return null;
        }

        public async Task<string> GetSalt(string userId)
        {
            await using var con = _connection.Clone();
            con.Open();

            await using var command =
                new MySqlCommand(
                    @"SELECT Auth_Salt FROM Auth WHERE Auth_UserId = @UserId",
                    con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
            }
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) return reader.GetString(0);

            throw new Exception("no user with given userId");
        }

        public async Task UpdateUser(User entity)
        {
            await using var con = _connection.Clone();
            con.Open();
            await using var command = new MySqlCommand(
                @"UPDATE `User` SET `User_Username`=@Username,`User_Nickname`=@Password,`User_ProfilePicURL`= @ProfilePic WHERE User_id = @UserId",
                con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = entity.Id;
                command.Parameters.Add("@Username", MySqlDbType.VarChar).Value = entity.Username;
                command.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Nickname;
                command.Parameters.Add("@ProfilePic", MySqlDbType.VarChar).Value = entity.ProfilePicUrl;
            }
            command.ExecuteNonQuery();
        }

        public async Task RemoveName(string userId)
        {
            await using var con = _connection.Clone();
            con.Open();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newRandomName = new(Enumerable.Repeat(chars, 16)
                .Select(s => s[_random.Next(s.Length)]).ToArray());

            await using var command =
                new MySqlCommand(@"UPDATE User SET User_Username = @RandomName WHERE User_id = @UserId", con);
            {
                command.Parameters.Add("@UserId", MySqlDbType.VarChar).Value = userId;
                command.Parameters.Add("@RandomName", MySqlDbType.VarChar).Value = newRandomName;
            }
            command.ExecuteNonQuery();
        }
    }
}